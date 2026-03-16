import { useState } from "react";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { holidayApi } from "../services/api";
import type { Holiday } from "../types/api";
import { formatDateDisplay } from "../utils/time";

interface HolidaysPageProps {
  token: string;
  holidays: Holiday[];
  onHolidaysChange: (holidays: Holiday[]) => void;
}

export default function HolidaysPage({ token, holidays, onHolidaysChange }: HolidaysPageProps) {
  const [editingId, setEditingId] = useState<string | null>(null);
  const [date, setDate] = useState("");
  const [description, setDescription] = useState("");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function loadHolidays() {
    if (!token) {
      return;
    }

    try {
      setLoading(true);
      const data = await holidayApi.list(token);
      onHolidaysChange(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao listar feriados.");
    } finally {
      setLoading(false);
    }
  }

  function resetForm() {
    setEditingId(null);
    setDate("");
    setDescription("");
  }

  function startEdit(holiday: Holiday) {
    setEditingId(holiday.id);
    setDate(holiday.date.slice(0, 10));
    setDescription(holiday.description);
  }

  async function submitHoliday() {
    if (!token || !date || !description) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setMessage(null);
      if (editingId) {
        await holidayApi.update(editingId, { date, description }, token);
        setMessage("Feriado atualizado.");
      } else {
        await holidayApi.create({ date, description }, token);
        setMessage("Feriado cadastrado.");
      }
      resetForm();
      await loadHolidays();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao salvar feriado.");
    } finally {
      setLoading(false);
    }
  }

  async function deleteHoliday(id: string) {
    if (!token || !window.confirm("Deseja excluir este feriado?")) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setMessage(null);
      await holidayApi.remove(id, token);
      setMessage("Feriado excluído.");
      if (editingId === id) {
        resetForm();
      }
      await loadHolidays();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao excluir feriado.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 2 }}>
        Feriados
      </Typography>

      <Card sx={{ mb: 2 }}>
        <CardContent>
          <Grid container spacing={2}>
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                type="date"
                label="Data"
                InputLabelProps={{ shrink: true }}
                value={date}
                onChange={(e) => setDate(e.target.value)}
              />
            </Grid>
            <Grid item xs={12} md={8}>
              <TextField
                fullWidth
                label="Descrição"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </Grid>
          </Grid>

          <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
            <Button variant="contained" disabled={loading || !token} onClick={submitHoliday}>
              {editingId ? "Salvar alteração" : "Salvar"}
            </Button>
            <Button variant="text" disabled={loading || !editingId} onClick={resetForm}>
              Cancelar edição
            </Button>
            <Button variant="outlined" disabled={loading || !token} onClick={loadHolidays}>
              Atualizar lista
            </Button>
          </Stack>
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          <Typography variant="h6" sx={{ mb: 1 }}>
            Lista de feriados
          </Typography>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Data</TableCell>
                <TableCell>Descrição</TableCell>
                <TableCell>Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {holidays.map((holiday) => (
                <TableRow key={holiday.id}>
                  <TableCell>{formatDateDisplay(holiday.date)}</TableCell>
                  <TableCell>{holiday.description}</TableCell>
                  <TableCell>
                    <Stack direction="row" spacing={1}>
                      <Button size="small" variant="outlined" onClick={() => startEdit(holiday)}>
                        Editar
                      </Button>
                      <Button size="small" color="error" variant="outlined" onClick={() => deleteHoliday(holiday.id)}>
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
      </Box>
    </Box>
  );
}
