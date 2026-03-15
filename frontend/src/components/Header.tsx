import LogoutIcon from "@mui/icons-material/Logout";
import {
  AppBar,
  Box,
  Button,
  Chip,
  Tab,
  Tabs,
  Toolbar,
  Typography,
} from "@mui/material";

export type AppPage = "dashboard" | "auth" | "employers" | "employees" | "holidays" | "timecards" | "summary" | "users";

interface HeaderProps {
  activePage: AppPage;
  onPageChange: (page: AppPage) => void;
  token: string;
  onLogout: () => void;
}

const tabs: { value: AppPage; label: string }[] = [
  { value: "dashboard", label: "Dashboard" },
  { value: "auth", label: "Autenticação" },
  { value: "employers", label: "Empresas" },
  { value: "employees", label: "Funcionários" },
  { value: "holidays", label: "Feriados" },
  { value: "timecards", label: "Apuração" },
  { value: "summary", label: "Resumo" },
  { value: "users", label: "Usuários" },
];

export default function Header({ activePage, onPageChange, token, onLogout }: HeaderProps) {
  return (
    <AppBar position="sticky" sx={{ bgcolor: "primary.main" }}>
      <Toolbar sx={{ gap: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 700 }}>
          Apuração de Ponto Simples
        </Typography>
        <Tabs
          value={activePage}
          onChange={(_, value: AppPage) => onPageChange(value)}
          textColor="inherit"
          indicatorColor="secondary"
          sx={{ minHeight: 48, "& .MuiTab-root": { minHeight: 48 } }}
        >
          {tabs.map((tab) => (
            <Tab key={tab.value} value={tab.value} label={tab.label} />
          ))}
        </Tabs>
        <Box sx={{ flexGrow: 1 }} />
        <Chip
          size="small"
          color={token ? "success" : "default"}
          label={token ? "JWT ativo" : "Sem login"}
          sx={{ color: "white", borderColor: "white" }}
          variant="outlined"
        />
        <Button color="inherit" startIcon={<LogoutIcon />} onClick={onLogout} disabled={!token}>
          Sair
        </Button>
      </Toolbar>
    </AppBar>
  );
}
