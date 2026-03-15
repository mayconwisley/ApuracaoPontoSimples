import { Box, Card, CardContent, Chip, Grid, Typography } from "@mui/material";

interface DashboardProps {
  employeesCount: number;
  holidaysCount: number;
  tokenConfigured: boolean;
}

export default function Dashboard({
  employeesCount,
  holidaysCount,
  tokenConfigured,
}: DashboardProps) {
  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 2 }}>
        Visão geral
      </Typography>
      <Grid container spacing={2}>
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="overline">Colaboradores</Typography>
              <Typography variant="h4" sx={{ mt: 1, fontWeight: 700 }}>
                {employeesCount}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Funcionários cadastrados
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="overline">Feriados</Typography>
              <Typography variant="h4" sx={{ mt: 1, fontWeight: 700 }}>
                {holidaysCount}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Eventos configurados
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="overline">Autenticação</Typography>
              <Typography variant="h6" sx={{ mt: 1 }}>
                Token JWT
              </Typography>
              <Chip
                size="small"
                label={tokenConfigured ? "Conectado" : "Não autenticado"}
                color={tokenConfigured ? "success" : "default"}
                sx={{ mt: 1 }}
              />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
