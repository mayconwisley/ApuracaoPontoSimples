import { useEffect, useMemo, useState } from "react";
import { Box, Container } from "@mui/material";
import Header, { type AppPage } from "./components/Header";
import Dashboard from "./pages/Dashboard";
import AuthPage from "./pages/AuthPage";
import EmployersPage from "./pages/EmployersPage";
import EmployeesPage from "./pages/EmployeesPage";
import HolidaysPage from "./pages/HolidaysPage";
import TimeCardsPage from "./pages/TimeCardsPage";
import TimeCardSummaryPage from "./pages/TimeCardSummaryPage";
import UsersPage from "./pages/UsersPage";
import type { Employee, Employer, Holiday } from "./types/api";
import { employeeApi, employerApi, holidayApi } from "./services/api";

export default function App() {
  const [page, setPage] = useState<AppPage>("dashboard");
  const [token, setToken] = useState(localStorage.getItem("jwt_token") ?? "");
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [employers, setEmployers] = useState<Employer[]>([]);
  const [holidays, setHolidays] = useState<Holiday[]>([]);

  useEffect(() => {
    if (token) {
      localStorage.setItem("jwt_token", token);
    } else {
      localStorage.removeItem("jwt_token");
    }
  }, [token]);

  useEffect(() => {
    async function loadData() {
      if (!token) {
        setEmployees([]);
        setEmployers([]);
        setHolidays([]);
        return;
      }

      try {
        const [employeeData, employerData, holidayData] = await Promise.all([
          employeeApi.list(token),
          employerApi.list(token),
          holidayApi.list(token),
        ]);
        setEmployees(employeeData);
        setEmployers(employerData);
        setHolidays(holidayData);
      } catch {
        // Mantém uso simples; erros detalhados aparecem nas telas de operação.
      }
    }
    void loadData();
  }, [token]);

  const currentPage = useMemo(() => {
    switch (page) {
      case "dashboard":
        return (
          <Dashboard
            employeesCount={employees.length}
            holidaysCount={holidays.length}
            tokenConfigured={!!token}
          />
        );
      case "auth":
        return <AuthPage token={token} onToken={setToken} />;
      case "employers":
        return <EmployersPage token={token} employers={employers} onEmployersChange={setEmployers} />;
      case "employees":
        return (
          <EmployeesPage
            token={token}
            employees={employees}
            employers={employers}
            onEmployeesChange={setEmployees}
          />
        );
      case "holidays":
        return <HolidaysPage token={token} holidays={holidays} onHolidaysChange={setHolidays} />;
      case "timecards":
        return <TimeCardsPage token={token} />;
      case "summary":
        return <TimeCardSummaryPage token={token} employees={employees} />;
      case "users":
        return <UsersPage token={token} />;
      default:
        return null;
    }
  }, [employees, employers, holidays, page, token]);

  return (
    <Box minHeight="100vh" bgcolor="background.default" color="text.primary">
      <Header
        activePage={page}
        onPageChange={setPage}
        token={token}
        onLogout={() => setToken("")}
      />
      <Container maxWidth="xl" sx={{ py: 3 }}>
        {currentPage}
      </Container>
    </Box>
  );
}
