import { useEffect, useMemo, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Divider,
  Grid,
  MenuItem,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { employeeApi } from "../services/api";
import type { Employee, Employer } from "../types/api";
import { fromDotnetTimeSpan, toDotnetTimeSpan } from "../utils/time";

interface EmployeesPageProps {
  token: string;
  employees: Employee[];
  employers: Employer[];
  onEmployeesChange: (employees: Employee[]) => void;
}

const defaultSchedule = {
  dailyHours: "08:00",
  dailyLimit: "09:00",
  saturdayHours: "04:00",
  toleranceEntry: "00:10",
  toleranceExit: "00:10",
  nightStart: "22:00",
  nightEnd: "05:00",
  weeklyHours: "44:00",
  saturdayCountsAsBank: true,
  useDailyHoursAsY13: false,
};

export default function EmployeesPage({ token, employees, employers, onEmployeesChange }: EmployeesPageProps) {
  const [editingId, setEditingId] = useState<string | null>(null);
  const [name, setName] = useState("");
  const [pis, setPis] = useState("");
  const [admissionDate, setAdmissionDate] = useState("");
  const [employerId, setEmployerId] = useState("");
  const [schedule, setSchedule] = useState({ ...defaultSchedule });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const canSave = useMemo(() => !!token && !!name && !!employerId, [employerId, name, token]);

  useEffect(() => {
    if (!employerId && employers.length > 0) {
      setEmployerId(employers[0].id);
    }
  }, [employerId, employers]);

  function resetForm() {
    setEditingId(null);
    setName("");
    setPis("");
    setAdmissionDate("");
    setSchedule({ ...defaultSchedule });
  }

  async function startEdit(employee: Employee) {
    if (!token) {
      return;
    }

    try {
      setLoading(true);
      setMessage(null);
      setError(null);
      const details = await employeeApi.getById(employee.id, token);
      setEditingId(details.id);
      setName(details.name);
      setPis(details.pis ?? "");
      setAdmissionDate(details.admissionDate ?? "");
      setEmployerId(details.employerId);
      if (details.schedule) {
        setSchedule({
          dailyHours: fromDotnetTimeSpan(details.schedule.dailyHours),
          dailyLimit: fromDotnetTimeSpan(details.schedule.dailyLimit),
          saturdayHours: fromDotnetTimeSpan(details.schedule.saturdayHours),
          toleranceEntry: fromDotnetTimeSpan(details.schedule.toleranceEntry),
          toleranceExit: fromDotnetTimeSpan(details.schedule.toleranceExit),
          nightStart: fromDotnetTimeSpan(details.schedule.nightStart),
          nightEnd: fromDotnetTimeSpan(details.schedule.nightEnd),
          weeklyHours: fromDotnetTimeSpan(details.schedule.weeklyHours),
          saturdayCountsAsBank: details.schedule.saturdayCountsAsBank,
          useDailyHoursAsY13: details.schedule.useDailyHoursAsY13,
        });
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar funcionário para edição.");
    } finally {
      setLoading(false);
    }
  }

  async function loadEmployees() {
    if (!token) {
      return;
    }
    try {
      setLoading(true);
      const data = await employeeApi.list(token);
      onEmployeesChange(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar funcionários.");
    } finally {
      setLoading(false);
    }
  }

  function buildSchedulePayload() {
    return {
      dailyHours: toDotnetTimeSpan(schedule.dailyHours),
      dailyLimit: toDotnetTimeSpan(schedule.dailyLimit),
      saturdayHours: toDotnetTimeSpan(schedule.saturdayHours),
      toleranceEntry: toDotnetTimeSpan(schedule.toleranceEntry),
      toleranceExit: toDotnetTimeSpan(schedule.toleranceExit),
      nightStart: toDotnetTimeSpan(schedule.nightStart),
      nightEnd: toDotnetTimeSpan(schedule.nightEnd),
      weeklyHours: toDotnetTimeSpan(schedule.weeklyHours),
      saturdayCountsAsBank: schedule.saturdayCountsAsBank,
      useDailyHoursAsY13: schedule.useDailyHoursAsY13,
    };
  }

  async function submitEmployee() {
    if (!canSave) {
      return;
    }

    try {
      setLoading(true);
      setMessage(null);
      setError(null);

      const payload = {
        name,
        pis: pis || null,
        admissionDate: admissionDate || null,
        employerId,
        schedule: buildSchedulePayload(),
      };

      if (editingId) {
        await employeeApi.update(editingId, payload, token);
        setMessage("Funcionário atualizado.");
      } else {
        await employeeApi.create(payload, token);
        setMessage("Funcionário cadastrado.");
      }

      resetForm();
      await loadEmployees();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao salvar funcionário.");
    } finally {
      setLoading(false);
    }
  }

  async function deleteEmployee(id: string) {
    if (!token || !window.confirm("Deseja excluir este funcionário?")) {
      return;
    }

    try {
      setLoading(true);
      setMessage(null);
      setError(null);
      await employeeApi.remove(id, token);
      setMessage("Funcionário excluído.");
      if (editingId === id) {
        resetForm();
      }
      await loadEmployees();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao excluir funcionário.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 2 }}>
        Funcionários e jornada
      </Typography>

      <Card sx={{ mb: 2 }}>
        <CardContent>
          <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>
            Funcionário
          </Typography>
          <Grid container spacing={2}>
            <Grid item xs={12} md={4}>
              <TextField fullWidth label="Nome" value={name} onChange={(e) => setName(e.target.value)} />
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField fullWidth label="PIS" value={pis} onChange={(e) => setPis(e.target.value)} />
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                type="date"
                label="Data admissão"
                InputLabelProps={{ shrink: true }}
                value={admissionDate}
                onChange={(e) => setAdmissionDate(e.target.value)}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField select fullWidth label="Empregador" value={employerId} onChange={(e) => setEmployerId(e.target.value)}>
                {employers.length === 0 && <MenuItem value="">Nenhum empregador cadastrado</MenuItem>}
                {employers.map((employer) => (
                  <MenuItem key={employer.id} value={employer.id}>
                    {employer.name} ({employer.id})
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
          </Grid>

          <Divider sx={{ my: 2 }} />

          <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>
            Configuração de jornada
          </Typography>
          <Grid container spacing={2}>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                label="Horas dia"
                value={schedule.dailyHours}
                onChange={(e) => setSchedule({ ...schedule, dailyHours: e.target.value })}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                label="Limite diário"
                value={schedule.dailyLimit}
                onChange={(e) => setSchedule({ ...schedule, dailyLimit: e.target.value })}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                label="Horas sábado"
                value={schedule.saturdayHours}
                onChange={(e) => setSchedule({ ...schedule, saturdayHours: e.target.value })}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                label="Horas semana"
                value={schedule.weeklyHours}
                onChange={(e) => setSchedule({ ...schedule, weeklyHours: e.target.value })}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                label="Tol. entrada"
                value={schedule.toleranceEntry}
                onChange={(e) => setSchedule({ ...schedule, toleranceEntry: e.target.value })}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                label="Tol. saída"
                value={schedule.toleranceExit}
                onChange={(e) => setSchedule({ ...schedule, toleranceExit: e.target.value })}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                label="Início noturno"
                value={schedule.nightStart}
                onChange={(e) => setSchedule({ ...schedule, nightStart: e.target.value })}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                label="Fim noturno"
                value={schedule.nightEnd}
                onChange={(e) => setSchedule({ ...schedule, nightEnd: e.target.value })}
              />
            </Grid>
          </Grid>

          <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
            <Button variant="contained" disabled={loading || !canSave} onClick={submitEmployee}>
              {editingId ? "Salvar alteração" : "Salvar funcionário"}
            </Button>
            <Button variant="text" disabled={loading || !editingId} onClick={resetForm}>
              Cancelar edição
            </Button>
            <Button variant="outlined" disabled={loading || !token} onClick={loadEmployees}>
              Atualizar lista
            </Button>
          </Stack>
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          <Typography variant="h6" sx={{ mb: 1 }}>
            Lista de funcionários
          </Typography>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Nome</TableCell>
                <TableCell>PIS</TableCell>
                <TableCell>Admissão</TableCell>
                <TableCell>Employer</TableCell>
                <TableCell>Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {employees.map((employee) => (
                <TableRow key={employee.id}>
                  <TableCell>{employee.name}</TableCell>
                  <TableCell>{employee.pis ?? "-"}</TableCell>
                  <TableCell>{employee.admissionDate ?? "-"}</TableCell>
                  <TableCell>{employee.employerId}</TableCell>
                  <TableCell>
                    <Stack direction="row" spacing={1}>
                      <Button size="small" variant="outlined" onClick={() => startEdit(employee)}>
                        Editar
                      </Button>
                      <Button size="small" color="error" variant="outlined" onClick={() => deleteEmployee(employee.id)}>
                        Excluir
                      </Button>
                    </Stack>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <Box sx={{ mt: 2 }}>
        {message && <Alert severity="success">{message}</Alert>}
        {error && <Alert severity="error">{error}</Alert>}
        {!token && <Alert severity="info">Faça login para habilitar operações.</Alert>}
        {token && employers.length === 0 && (
          <Alert severity="warning">Cadastre uma empresa na tela Empresas antes de criar funcionários.</Alert>
        )}
      </Box>
    </Box>
  );
}
