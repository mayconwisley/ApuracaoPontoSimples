import { useState } from "react";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { authApi } from "../services/api";

interface AuthPageProps {
  token: string;
  onToken: (token: string) => void;
}

export default function AuthPage({ token, onToken }: AuthPageProps) {
  const [email, setEmail] = useState("admin@local");
  const [password, setPassword] = useState("Senha@123");
  const [fullName, setFullName] = useState("Administrador");
  const [role, setRole] = useState("Admin");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function run(action: () => Promise<{ token: string }>) {
    try {
      setLoading(true);
      setError(null);
      setMessage(null);
      const response = await action();
      onToken(response.token);
      setMessage("Operação executada com sucesso.");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Falha na autenticação.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 2 }}>
        Autenticação e perfis
      </Typography>
      <Grid container spacing={2}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" sx={{ mb: 2 }}>
                Bootstrap / Login
              </Typography>
              <Stack spacing={2}>
                <TextField label="Nome completo" value={fullName} onChange={(e) => setFullName(e.target.value)} />
                <TextField label="E-mail" value={email} onChange={(e) => setEmail(e.target.value)} />
                <TextField
                  label="Senha"
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
                <Stack direction="row" spacing={1}>
                  <Button
                    variant="contained"
                    disabled={loading}
                    onClick={() =>
                      run(() =>
                        authApi.bootstrap({
                          email,
                          password,
                          fullName,
                        })
                      )
                    }
                  >
                    Bootstrap
                  </Button>
                  <Button variant="outlined" disabled={loading} onClick={() => run(() => authApi.login({ email, password }))}>
                    Login
                  </Button>
                </Stack>
              </Stack>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" sx={{ mb: 2 }}>
                Criar usuário
              </Typography>
              <Stack spacing={2}>
                <TextField label="Nome completo" value={fullName} onChange={(e) => setFullName(e.target.value)} />
                <TextField label="E-mail" value={email} onChange={(e) => setEmail(e.target.value)} />
                <TextField
                  label="Senha"
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
                <TextField select label="Perfil" value={role} onChange={(e) => setRole(e.target.value)}>
                  <MenuItem value="Admin">Admin</MenuItem>
                  <MenuItem value="Gestor">Gestor</MenuItem>
                  <MenuItem value="Usuario">Usuário</MenuItem>
                </TextField>
                <Button
                  variant="contained"
                  disabled={loading || !token}
                  onClick={() =>
                    run(() =>
                      authApi.register(
                        {
                          email,
                          password,
                          fullName,
                          role,
                        },
                        token
                      )
                    )
                  }
                >
                  Cadastrar usuário
                </Button>
                {!token && <Typography color="warning.main">Faça login como Admin para habilitar o cadastro.</Typography>}
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Box sx={{ mt: 2 }}>
        {message && <Alert severity="success">{message}</Alert>}
        {error && <Alert severity="error">{error}</Alert>}
      </Box>
    </Box>
  );
}
