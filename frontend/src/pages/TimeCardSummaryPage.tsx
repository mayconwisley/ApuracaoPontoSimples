import { useMemo, useState } from "react";
import {
  Autocomplete,
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import type { Employee, TimeCard } from "../types/api";
import { timeCardApi } from "../services/api";
import TimeCardSummaryPanel from "../components/TimeCardSummaryPanel";

interface TimeCardSummaryPageProps {
  token: string;
  employees: Employee[];
}

export default function TimeCardSummaryPage({ token, employees }: TimeCardSummaryPageProps) {
  const now = new Date();
  const [employeeId, setEmployeeId] = useState("");
  const [startDate, setStartDate] = useState(`${now.getFullYear()}-${`${now.getMonth() + 1}`.padStart(2, "0")}-01`);
  const [endDate, setEndDate] = useState(`${now.getFullYear()}-${`${now.getMonth() + 1}`.padStart(2, "0")}-${`${new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate()}`.padStart(2, "0")}`);
  const [result, setResult] = useState<TimeCard | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  const hasToken = useMemo(() => !!token, [token]);
  const selectedEmployee = useMemo(
    () => employees.find((employee) => employee.id === employeeId) ?? null,
    [employees, employeeId]
  );

  async function loadTimeCard() {
    if (!hasToken || !employeeId) {
      return;
    }
    if (!startDate || !endDate) {
      setError("Informe data inicial e final do período.");
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setMessage(null);
      const data = await timeCardApi.get(employeeId, startDate, endDate, token);
      setResult(data);
      setMessage("Resumo carregado.");
    } catch (err) {
      setResult(null);
      setError(err instanceof Error ? err.message : "Erro ao buscar resumo da apuração.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 2 }}>
        Resumo da Apuração
      </Typography>

      <Card sx={{ mb: 2 }}>
        <CardContent>
          <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
              <Autocomplete
                options={employees}
                value={selectedEmployee}
                getOptionLabel={(option) => `${option.name} (${option.id})`}
                onChange={(_, value) => setEmployeeId(value?.id ?? "")}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Empregado"
                    placeholder="Selecione o empregado"
                  />
                )}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                type="date"
                label="Data inicial"
                InputLabelProps={{ shrink: true }}
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
              />
            </Grid>
            <Grid item xs={6} md={3}>
              <TextField
                fullWidth
                type="date"
                label="Data final"
                InputLabelProps={{ shrink: true }}
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
              />
            </Grid>
          </Grid>
          <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
            <Button variant="contained" disabled={loading || !hasToken || !employeeId} onClick={loadTimeCard}>
              Carregar resumo
            </Button>
          </Stack>
        </CardContent>
      </Card>

      {result && <TimeCardSummaryPanel timeCard={result} employeeName={selectedEmployee?.name} />}

      <Box sx={{ mt: 2 }}>
        {message && <Alert severity="success">{message}</Alert>}
        {error && <Alert severity="error">{error}</Alert>}
        {!hasToken && <Alert severity="info">Faça login para habilitar operações.</Alert>}
      </Box>
    </Box>
  );
}
