import { useState } from "react";
import { Alert, Box, Button, Card, CardContent, MenuItem, Stack, TextField, Typography } from "@mui/material";
import { authApi } from "../services/api";

interface UsersPageProps {
  token: string;
}

export default function UsersPage({ token }: UsersPageProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [fullName, setFullName] = useState("");
  const [role, setRole] = useState("Gestor");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function createUser() {
    if (!token) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setMessage(null);
      await authApi.register({ email, password, fullName, role }, token);
      setMessage("Usuário cadastrado com sucesso.");
      setEmail("");
      setPassword("");
      setFullName("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao cadastrar usuário.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 2 }}>
        Usuários e perfis
      </Typography>

      <Card>
        <CardContent>
          <Stack spacing={2}>
            <TextField label="Nome completo" value={fullName} onChange={(e) => setFullName(e.target.value)} />
            <TextField label="E-mail" value={email} onChange={(e) => setEmail(e.target.value)} />
            <TextField label="Senha" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
            <TextField select label="Perfil" value={role} onChange={(e) => setRole(e.target.value)}>
              <MenuItem value="Admin">Admin</MenuItem>
              <MenuItem value="Gestor">Gestor</MenuItem>
              <MenuItem value="Usuario">Usuário</MenuItem>
            </TextField>
            <Button variant="contained" disabled={loading || !token} onClick={createUser}>
              Cadastrar usuário
            </Button>
            {!token && (
              <Alert severity="info">
                Para cadastro de usuários, autentique-se primeiro na tela de autenticação.
              </Alert>
            )}
          </Stack>
        </CardContent>
      </Card>

      <Box sx={{ mt: 2 }}>
        {message && <Alert severity="success">{message}</Alert>}
        {error && <Alert severity="error">{error}</Alert>}
      </Box>
    </Box>
  );
}
