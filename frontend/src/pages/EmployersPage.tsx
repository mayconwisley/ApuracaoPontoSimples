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
import { employerApi } from "../services/api";
import type { Employer } from "../types/api";

interface EmployersPageProps {
  token: string;
  employers: Employer[];
  onEmployersChange: (employers: Employer[]) => void;
}

export default function EmployersPage({ token, employers, onEmployersChange }: EmployersPageProps) {
  const [editingId, setEditingId] = useState<string | null>(null);
  const [name, setName] = useState("");
  const [cnpj, setCnpj] = useState("");
  const [address, setAddress] = useState("");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function loadEmployers() {
    if (!token) {
      return;
    }
    try {
      setLoading(true);
      const data = await employerApi.list(token);
      onEmployersChange(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar empresas.");
    } finally {
      setLoading(false);
    }
  }

  function resetForm() {
    setEditingId(null);
    setName("");
    setCnpj("");
    setAddress("");
  }

  function startEdit(employer: Employer) {
    setEditingId(employer.id);
    setName(employer.name);
    setCnpj(employer.cnpj ?? "");
    setAddress(employer.address ?? "");
  }

  async function submitEmployer() {
    if (!token || !name) {
      return;
    }

    try {
      setLoading(true);
      setMessage(null);
      setError(null);
      if (editingId) {
        await employerApi.update(editingId, { name, cnpj: cnpj || null, address: address || null }, token);
        setMessage("Empresa atualizada.");
      } else {
        await employerApi.create({ name, cnpj: cnpj || null, address: address || null }, token);
        setMessage("Empresa cadastrada.");
      }
      resetForm();
      await loadEmployers();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao salvar empresa.");
    } finally {
      setLoading(false);
    }
  }

  async function deleteEmployer(id: string) {
    if (!token || !window.confirm("Deseja excluir esta empresa?")) {
      return;
    }

    try {
      setLoading(true);
      setMessage(null);
      setError(null);
      await employerApi.remove(id, token);
      setMessage("Empresa excluída.");
      if (editingId === id) {
        resetForm();
      }
      await loadEmployers();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao excluir empresa.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 2 }}>
        Empresas
      </Typography>

      <Card sx={{ mb: 2 }}>
        <CardContent>
          <Grid container spacing={2}>
            <Grid item xs={12} md={4}>
              <TextField fullWidth label="Nome da empresa" value={name} onChange={(e) => setName(e.target.value)} />
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField fullWidth label="CNPJ" value={cnpj} onChange={(e) => setCnpj(e.target.value)} />
            </Grid>
            <Grid item xs={12} md={4}>
              <TextField fullWidth label="Endereço" value={address} onChange={(e) => setAddress(e.target.value)} />
            </Grid>
          </Grid>
          <Stack direction="row" spacing={1} sx={{ mt: 2 }}>
            <Button variant="contained" disabled={loading || !token || !name} onClick={submitEmployer}>
              {editingId ? "Salvar alteração" : "Salvar empresa"}
            </Button>
            <Button variant="text" disabled={loading || !editingId} onClick={resetForm}>
              Cancelar edição
            </Button>
            <Button variant="outlined" disabled={loading || !token} onClick={loadEmployers}>
              Atualizar lista
            </Button>
          </Stack>
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          <Typography variant="h6" sx={{ mb: 1 }}>
            Lista de empresas
          </Typography>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Nome</TableCell>
                <TableCell>CNPJ</TableCell>
                <TableCell>Endereço</TableCell>
                <TableCell>Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {employers.map((employer) => (
                <TableRow key={employer.id}>
                  <TableCell>{employer.name}</TableCell>
                  <TableCell>{employer.cnpj ?? "-"}</TableCell>
                  <TableCell>{employer.address ?? "-"}</TableCell>
                  <TableCell>
                    <Stack direction="row" spacing={1}>
                      <Button size="small" variant="outlined" onClick={() => startEdit(employer)}>
                        Editar
                      </Button>
                      <Button size="small" color="error" variant="outlined" onClick={() => deleteEmployer(employer.id)}>
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
