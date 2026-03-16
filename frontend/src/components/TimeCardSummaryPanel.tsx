import {
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Grid,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import type { TimeCard } from "../types/api";
import { fromDotnetTimeSpan, formatDateDisplay } from "../utils/time";

const weekdayFormatter = new Intl.DateTimeFormat("pt-BR", { weekday: "long" });

function getWeekdayLabel(dateIso: string): string {
  const [year, month, day] = dateIso.split("-").map(Number);
  const date = new Date(year, month - 1, day);
  const label = weekdayFormatter.format(date);
  return label.charAt(0).toUpperCase() + label.slice(1);
}
interface TimeCardSummaryPanelProps {
  timeCard: TimeCard;
  employeeName?: string;
}

function exportSummaryCsv(timeCard: TimeCard, employeeName?: string) {
  const days = [...(timeCard.days ?? [])].sort((a, b) => a.date.localeCompare(b.date));
  const header = [
    "Data",
    "Dia Semana",
    "Ent1",
    "Sai1",
    "Ent2",
    "Sai2",
    "Ent3",
    "Sai3",
    "APU",
    "BANCO",
    "EXT",
    "DRS/FER",
    "AD.NOT",
  ];

  const lines = [
    [`Colaborador`, employeeName ?? timeCard.employeeId],
    [`Período`, `${timeCard.startDate ?? "-"} até ${timeCard.endDate ?? "-"}`],
    [],
    header,
    ...days.map((day) => [
      formatDateDisplay(day.date),
      getWeekdayLabel(day.date).toLowerCase(),
      fromDotnetTimeSpan(day.interval1?.start),
      fromDotnetTimeSpan(day.interval1?.end),
      fromDotnetTimeSpan(day.interval2?.start),
      fromDotnetTimeSpan(day.interval2?.end),
      fromDotnetTimeSpan(day.interval3?.start),
      fromDotnetTimeSpan(day.interval3?.end),
      fromDotnetTimeSpan(day.calculation?.apuracao),
      fromDotnetTimeSpan(day.calculation?.bancoHoras),
      fromDotnetTimeSpan(day.calculation?.horasExtras),
      fromDotnetTimeSpan(day.calculation?.horasDsrFeriado),
      fromDotnetTimeSpan(day.calculation?.horasNoturnas),
    ]),
    [],
    ["Apuração total", fromDotnetTimeSpan(timeCard.totals?.apuracaoTotal)],
    ["Banco total", fromDotnetTimeSpan(timeCard.totals?.bancoHorasTotal)],
    ["Horas extras", fromDotnetTimeSpan(timeCard.totals?.horasExtrasTotal)],
    ["DRS/Feriado", fromDotnetTimeSpan(timeCard.totals?.horasDsrFeriadoTotal)],
    ["Ad. Noturno", fromDotnetTimeSpan(timeCard.totals?.horasNoturnasTotal)],
    ["Horas positivas", fromDotnetTimeSpan(timeCard.totals?.horasPositivasTotal)],
    ["Horas negativas", fromDotnetTimeSpan(timeCard.totals?.horasNegativasTotal)],
  ];

  const csv = lines
    .map((line) => line.map((value) => `"${String(value ?? "").replace(/"/g, '""')}"`).join(";"))
    .join("\n");

  const blob = new Blob(["\uFEFF" + csv], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `resumo-apuracao-${timeCard.startDate ?? "periodo"}-${timeCard.endDate ?? "periodo"}.csv`;
  a.click();
  URL.revokeObjectURL(url);
}

function printSummaryPdf() {
  window.print();
}

export default function TimeCardSummaryPanel({ timeCard, employeeName }: TimeCardSummaryPanelProps) {
  const days = [...(timeCard.days ?? [])].sort((a, b) => a.date.localeCompare(b.date));

  return (
    <Card>
      <CardContent>
        <Typography variant="h6">Resumo da apuração</Typography>
        <Typography variant="body2">Colaborador: {employeeName ?? "-"}</Typography>
        <Typography variant="body2">
          Período: {(timeCard.startDate ?? "-")} até {(timeCard.endDate ?? "-")}
        </Typography>
        <Box sx={{ mt: 2, display: "flex", gap: 1 }}>
          <Button variant="outlined" size="small" onClick={() => exportSummaryCsv(timeCard, employeeName)}>
            Baixar Excel
          </Button>
          <Button variant="outlined" size="small" onClick={printSummaryPdf}>
            Baixar PDF
          </Button>
        </Box>

        <TableContainer sx={{ mt: 2 }}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Data</TableCell>
                <TableCell>Dia semana</TableCell>
                <TableCell>Ent1</TableCell>
                <TableCell>Sai1</TableCell>
                <TableCell>Ent2</TableCell>
                <TableCell>Sai2</TableCell>
                <TableCell>Ent3</TableCell>
                <TableCell>Sai3</TableCell>
                <TableCell>APU</TableCell>
                <TableCell>BANCO</TableCell>
                <TableCell>EXT</TableCell>
                <TableCell>DRS/FER</TableCell>
                <TableCell>AD.NOT</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {days.map((day) => (
                <TableRow key={day.id}>
                  <TableCell>{formatDateDisplay(day.date)}</TableCell>
                  <TableCell>
                    <Stack direction="row" spacing={1} alignItems="center">
                      <span>{getWeekdayLabel(day.date).toLowerCase()}</span>
                      {day.holidayType === 2 && <Chip size="small" color="success" label="Feriado" />}
                    </Stack>
                  </TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.interval1?.start)}</TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.interval1?.end)}</TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.interval2?.start)}</TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.interval2?.end)}</TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.interval3?.start)}</TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.interval3?.end)}</TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.calculation?.apuracao)}</TableCell>
                  <TableCell
                    sx={{
                      color: day.calculation?.bancoHoras?.startsWith("-")
                        ? "error.main"
                        : "inherit",
                    }}
                  >
                    {fromDotnetTimeSpan(day.calculation?.bancoHoras)}
                  </TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.calculation?.horasExtras)}</TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.calculation?.horasDsrFeriado)}</TableCell>
                  <TableCell>{fromDotnetTimeSpan(day.calculation?.horasNoturnas)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>

        <Grid container spacing={1} sx={{ mt: 2 }}>
          <Grid item xs={12} md={3}>
            <Typography variant="body2">Apuração total: {fromDotnetTimeSpan(timeCard.totals?.apuracaoTotal)}</Typography>
          </Grid>
          <Grid item xs={12} md={3}>
            <Typography variant="body2">Banco total: {fromDotnetTimeSpan(timeCard.totals?.bancoHorasTotal)}</Typography>
          </Grid>
          <Grid item xs={12} md={3}>
            <Typography variant="body2">Horas extras: {fromDotnetTimeSpan(timeCard.totals?.horasExtrasTotal)}</Typography>
          </Grid>
          <Grid item xs={12} md={3}>
            <Typography variant="body2">DRS/Feriado: {fromDotnetTimeSpan(timeCard.totals?.horasDsrFeriadoTotal)}</Typography>
          </Grid>
          <Grid item xs={12} md={3}>
            <Typography variant="body2">Ad. Noturno: {fromDotnetTimeSpan(timeCard.totals?.horasNoturnasTotal)}</Typography>
          </Grid>
          <Grid item xs={12} md={3}>
            <Typography variant="body2">Horas positivas: {fromDotnetTimeSpan(timeCard.totals?.horasPositivasTotal)}</Typography>
          </Grid>
          <Grid item xs={12} md={3}>
            <Typography variant="body2">Horas negativas: {fromDotnetTimeSpan(timeCard.totals?.horasNegativasTotal)}</Typography>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  );
}
