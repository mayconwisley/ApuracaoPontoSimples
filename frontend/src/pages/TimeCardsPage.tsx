import { memo, useCallback, useEffect, useMemo, useState } from "react";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Autocomplete,
  Alert,
  Box,
  Button,
  Chip,
  Card,
  CardContent,
  Grid,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { employeeApi, holidayApi, timeCardApi } from "../services/api";
import type { AbsenceType, DayCode, Employee, Holiday, HolidayType, TimeCard } from "../types/api";
import { formatDateDisplay, fromDotnetTimeSpan, toTimeSpan } from "../utils/time";

type DayForm = {
  date: string;
  code: DayCode;
  entrada1: string;
  saida1: string;
  entrada2: string;
  saida2: string;
  entrada3: string;
  saida3: string;
  holidayType: HolidayType;
  isSunday: boolean;
  isSaturday: boolean;
  absenceHours: string;
  absenceType: AbsenceType;
};

interface TimeCardsPageProps {
  token: string;
}

const weekdayFormatter = new Intl.DateTimeFormat("pt-BR", { weekday: "long" });

function getWeekdayLabel(dateIso: string): string {
  const [year, month, day] = dateIso.split("-").map(Number);
  const date = new Date(year, month - 1, day);
  const label = weekdayFormatter.format(date);
  return label.charAt(0).toUpperCase() + label.slice(1);
}

function getWeekdayFlags(dateIso: string): { isSaturday: boolean; isSunday: boolean } {
  const [year, month, day] = dateIso.split("-").map(Number);
  const date = new Date(year, month - 1, day);
  const weekDay = date.getDay();
  return { isSunday: weekDay === 0, isSaturday: weekDay === 6 };
}

function getDayCodeLabel(code: DayCode): string {
  switch (code) {
    case 1:
      return "CF";
    case 2:
      return "F";
    case 3:
      return "FR";
    case 4:
      return "AB";
    case 5:
      return "A";
    case 6:
      return "DS";
    default:
      return "N";
  }
}

function parseTimeToMinutes(value: string): number {
  if (!value) {
    return 0;
  }

  const [hoursPart, minutesPart] = value.split(":");
  const hours = Number(hoursPart);
  const minutes = Number(minutesPart ?? "0");
  if (Number.isNaN(hours) || Number.isNaN(minutes)) {
    return 0;
  }
  return hours * 60 + minutes;
}

function formatTimeInput(value: string): string {
  const digits = value.replace(/\D/g, "").slice(0, 4);
  if (digits.length <= 2) {
    return digits;
  }
  return `${digits.slice(0, 2)}:${digits.slice(2)}`;
}

function formatMinutesAsHour(minutes: number): string {
  const signal = minutes < 0 ? "-" : "";
  const absolute = Math.abs(minutes);
  const hh = `${Math.floor(absolute / 60)}`.padStart(2, "0");
  const mm = `${absolute % 60}`.padStart(2, "0");
  return `${signal}${hh}:${mm}`;
}

function calculateDayApuracao(day: DayForm): number {
  const entrada1 = parseTimeToMinutes(day.entrada1);
  const saida1 = parseTimeToMinutes(day.saida1);
  const entrada2 = parseTimeToMinutes(day.entrada2);
  const saida2 = parseTimeToMinutes(day.saida2);
  const entrada3 = parseTimeToMinutes(day.entrada3);
  const saida3 = parseTimeToMinutes(day.saida3);

  return (saida1 - entrada1) + (saida2 - entrada2) + (saida3 - entrada3);
}

function normalizeHolidayDate(value: string): string {
  return value.length >= 10 ? value.slice(0, 10) : value;
}

function parseIsoDateToLocal(value: string): Date | null {
  const [year, month, day] = value.split("-").map(Number);
  if (!year || !month || !day) {
    return null;
  }
  return new Date(year, month - 1, day);
}

function applyCalendarRules(days: DayForm[], holidays: Set<string>): DayForm[] {
  return days.map((day) => {
    const flags = getWeekdayFlags(day.date);
    const isHoliday = holidays.has(day.date);
    const isDsrDay = flags.isSunday || isHoliday;
    const holidayType: HolidayType = flags.isSunday ? 1 : isHoliday ? 2 : day.holidayType;

    return {
      ...day,
      isSunday: flags.isSunday,
      isSaturday: flags.isSaturday,
      code: isDsrDay ? 6 : day.code,
      holidayType,
    };
  });
}

function createPeriodDays(startDate: string, endDate: string): DayForm[] {
  if (!startDate || !endDate) {
    return [];
  }

  const start = parseIsoDateToLocal(startDate);
  const end = parseIsoDateToLocal(endDate);
  if (!start || !end || Number.isNaN(start.getTime()) || Number.isNaN(end.getTime()) || start > end) {
    return [];
  }

  const date = new Date(start);
  const items: DayForm[] = [];

  while (date <= end) {
    const dateIso = `${date.getFullYear()}-${`${date.getMonth() + 1}`.padStart(2, "0")}-${`${date.getDate()}`.padStart(2, "0")}`;
    const flags = getWeekdayFlags(dateIso);
    items.push({
      date: dateIso,
      code: 0,
      entrada1: "",
      saida1: "",
      entrada2: "",
      saida2: "",
      entrada3: "",
      saida3: "",
      holidayType: 0,
      isSunday: flags.isSunday,
      isSaturday: flags.isSaturday,
      absenceHours: "",
      absenceType: 0,
    });
    date.setDate(date.getDate() + 1);
  }

  return items;
}

function mapTimeCardToDayForms(timeCard: TimeCard): DayForm[] {
  const sourceDays = [...(timeCard.days ?? [])].sort((a, b) => a.date.localeCompare(b.date));

  return sourceDays.map((day) => ({
    date: day.date.slice(0, 10),
    code: (day.code ?? 0) as DayCode,
    entrada1: fromDotnetTimeSpan(day.interval1?.start),
    saida1: fromDotnetTimeSpan(day.interval1?.end),
    entrada2: fromDotnetTimeSpan(day.interval2?.start),
    saida2: fromDotnetTimeSpan(day.interval2?.end),
    entrada3: fromDotnetTimeSpan(day.interval3?.start),
    saida3: fromDotnetTimeSpan(day.interval3?.end),
    holidayType: (day.holidayType ?? 0) as HolidayType,
    isSunday: day.isSunday ?? false,
    isSaturday: day.isSaturday ?? false,
    absenceHours: fromDotnetTimeSpan(day.absence?.hours),
    absenceType: (day.absence?.type ?? 0) as AbsenceType,
  }));
}

function mergeDaysIntoPeriod(
  startDate: string,
  endDate: string,
  existingDays: DayForm[],
  holidaySet: Set<string>
): DayForm[] {
  const generated = createPeriodDays(startDate, endDate);
  const byDate = new Map(existingDays.map((day) => [day.date, day]));

  const merged = generated.map((day) => byDate.get(day.date) ?? day);
  return applyCalendarRules(merged, holidaySet);
}

interface DayRowProps {
  day: DayForm;
  index: number;
  onChange: (index: number, patch: Partial<DayForm>) => void;
  isCalendarDsr: boolean;
  isHoliday: boolean;
}

const DayAccordionRow = memo(function DayAccordionRow({ day, index, onChange, isCalendarDsr, isHoliday }: DayRowProps) {
  const weekday = getWeekdayLabel(day.date);
  const apuracao = calculateDayApuracao(day);
  const isSundayOnly = day.isSunday;
  const handleTimeChange = (field: keyof DayForm, value: string) => {
    onChange(index, { [field]: formatTimeInput(value) } as Partial<DayForm>);
  };

  return (
    <Accordion disableGutters TransitionProps={{ unmountOnExit: true }}>
      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
        <Box sx={{ width: "100%" }}>
          <Stack direction="row" spacing={1} alignItems="center">
            <Typography sx={{ fontWeight: 600 }}>{day.date}</Typography>
            {isSundayOnly ? (
              <>
                <Chip size="small" color="error" label="Domingo" />
                {isHoliday && <Chip size="small" color="success" label="Feriado" />}
              </>
            ) : (
              <>
                <Chip size="small" label={weekday} />
                {day.isSaturday && <Chip size="small" color="warning" label="Sábado" />}
                {isHoliday && <Chip size="small" color="success" label="Feriado" />}
                {isCalendarDsr && !isHoliday && <Chip size="small" color="info" label="DSR automático" />}
                <Chip size="small" variant="outlined" label={`Código ${getDayCodeLabel(day.code)}`} />
              </>
            )}
          </Stack>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            Apuração do dia: {formatMinutesAsHour(apuracao)}
          </Typography>
        </Box>
      </AccordionSummary>
      <AccordionDetails>
        <Grid container spacing={1}>
          <Grid item xs={6} md={2}>
            <TextField
              select
              fullWidth
              label="Código"
              value={day.code}
              disabled={isCalendarDsr}
              onChange={(e) => onChange(index, { code: Number(e.target.value) as DayCode })}
            >
              <MenuItem value={0}>N (Normal)</MenuItem>
              <MenuItem value={1}>CF</MenuItem>
              <MenuItem value={2}>F</MenuItem>
              <MenuItem value={3}>FR</MenuItem>
              <MenuItem value={4}>AB</MenuItem>
              <MenuItem value={5}>A</MenuItem>
              <MenuItem value={6}>DS</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              select
              fullWidth
              label="Feriado"
              value={day.holidayType}
              disabled={isCalendarDsr}
              onChange={(e) => onChange(index, { holidayType: Number(e.target.value) as HolidayType })}
            >
              <MenuItem value={0}>Nenhum</MenuItem>
              <MenuItem value={2}>Feriado</MenuItem>
              <MenuItem value={1}>DSR</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              fullWidth
              label="Entrada 1"
              placeholder="08:00"
              value={day.entrada1}
              onChange={(e) => handleTimeChange("entrada1", e.target.value)}
            />
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              fullWidth
              label="Saída 1"
              placeholder="12:00"
              value={day.saida1}
              onChange={(e) => handleTimeChange("saida1", e.target.value)}
            />
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              fullWidth
              label="Entrada 2"
              placeholder="13:00"
              value={day.entrada2}
              onChange={(e) => handleTimeChange("entrada2", e.target.value)}
            />
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              fullWidth
              label="Saída 2"
              placeholder="18:00"
              value={day.saida2}
              onChange={(e) => handleTimeChange("saida2", e.target.value)}
            />
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              fullWidth
              label="Entrada 3"
              value={day.entrada3}
              onChange={(e) => handleTimeChange("entrada3", e.target.value)}
            />
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              fullWidth
              label="Saída 3"
              value={day.saida3}
              onChange={(e) => handleTimeChange("saida3", e.target.value)}
            />
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              select
              fullWidth
              label="Ausência"
              value={day.absenceType}
              onChange={(e) => onChange(index, { absenceType: Number(e.target.value) as AbsenceType })}
            >
              <MenuItem value={0}>Nenhuma</MenuItem>
              <MenuItem value={1}>Falta</MenuItem>
              <MenuItem value={2}>Banco de horas</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={6} md={2}>
            <TextField
              fullWidth
              label="Horas ausência"
              placeholder="08:00"
              value={day.absenceHours}
              onChange={(e) => handleTimeChange("absenceHours", e.target.value)}
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              label="Dia da semana"
              value={weekday}
              InputProps={{ readOnly: true }}
            />
          </Grid>
        </Grid>
      </AccordionDetails>
    </Accordion>
  );
});

export default function TimeCardsPage({ token }: TimeCardsPageProps) {
  const now = new Date();
  const [employeeId, setEmployeeId] = useState("");
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [holidays, setHolidays] = useState<Holiday[]>([]);
  const [startDate, setStartDate] = useState(`${now.getFullYear()}-${`${now.getMonth() + 1}`.padStart(2, "0")}-01`);
  const [endDate, setEndDate] = useState(`${now.getFullYear()}-${`${now.getMonth() + 1}`.padStart(2, "0")}-${`${new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate()}`.padStart(2, "0")}`);
  const [days, setDays] = useState<DayForm[]>(() =>
    createPeriodDays(
      `${now.getFullYear()}-${`${now.getMonth() + 1}`.padStart(2, "0")}-01`,
      `${now.getFullYear()}-${`${now.getMonth() + 1}`.padStart(2, "0")}-${`${new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate()}`.padStart(2, "0")}`
    )
  );
  const [result, setResult] = useState<TimeCard | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  const hasToken = useMemo(() => !!token, [token]);
  const holidaySet = useMemo(
    () => new Set(holidays.map((holiday) => normalizeHolidayDate(holiday.date))),
    [holidays]
  );
  const selectedEmployee = useMemo(
    () => employees.find((employee) => employee.id === employeeId) ?? null,
    [employees, employeeId]
  );
  useEffect(() => {
    async function loadEmployees() {
      if (!token) {
        setEmployees([]);
        setEmployeeId("");
        return;
      }

      try {
        const [employeeData, holidayData] = await Promise.all([
          employeeApi.list(token),
          holidayApi.list(token),
        ]);
        setEmployees(employeeData);
        setHolidays(holidayData);
        if (!employeeId && employeeData.length > 0) {
          setEmployeeId(employeeData[0].id);
        }
      } catch {
        setEmployees([]);
        setHolidays([]);
      }
    }
    void loadEmployees();
  }, [token]);

  useEffect(() => {
    setDays((prev) => applyCalendarRules(prev, holidaySet));
  }, [holidaySet]);

  function regenerateDays() {
    setDays(applyCalendarRules(createPeriodDays(startDate, endDate), holidaySet));
  }

  const updateDay = useCallback((index: number, patch: Partial<DayForm>) => {
    setDays((prev) => {
      const next = [...prev];
      next[index] = { ...next[index], ...patch };
      return next;
    });
  }, []);

  async function submitTimeCard() {
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

      const payload = {
        employeeId,
        startDate,
        endDate,
        days: days.map((day) => ({
          date: day.date,
          code: day.code,
          entrada1: toTimeSpan(day.entrada1),
          saida1: toTimeSpan(day.saida1),
          entrada2: toTimeSpan(day.entrada2),
          saida2: toTimeSpan(day.saida2),
          entrada3: toTimeSpan(day.entrada3),
          saida3: toTimeSpan(day.saida3),
          holidayType: day.holidayType,
          isSunday: getWeekdayFlags(day.date).isSunday,
          isSaturday: getWeekdayFlags(day.date).isSaturday,
          absenceHours: toTimeSpan(day.absenceHours),
          absenceType: day.absenceType,
        })),
      };

      const saved = await timeCardApi.create(payload, token);
      setResult(saved);
      setMessage("Cartão processado e salvo. Consulte o detalhamento na tela Resumo da Apuração.");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao salvar cartão.");
    } finally {
      setLoading(false);
    }
  }

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
      if (data.startDate) {
        setStartDate(data.startDate.slice(0, 10));
      }
      if (data.endDate) {
        setEndDate(data.endDate.slice(0, 10));
      }
      setDays(mergeDaysIntoPeriod(startDate, endDate, mapTimeCardToDayForms(data), holidaySet));
      setMessage("Cartão carregado.");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao buscar cartão.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 2 }}>
        Apuração de ponto
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
            <Button variant="outlined" onClick={regenerateDays}>
              Gerar dias do mês
            </Button>
            <Button variant="contained" disabled={loading || !hasToken} onClick={submitTimeCard}>
              Processar e salvar
            </Button>
            <Button variant="outlined" disabled={loading || !hasToken} onClick={loadTimeCard}>
              Buscar existente
            </Button>
          </Stack>
        </CardContent>
      </Card>

      <Card sx={{ mb: 2 }}>
        <CardContent>
          <Typography variant="h6" sx={{ mb: 1 }}>
            Dias do mês
          </Typography>
          {days.map((day, index) => (
            <DayAccordionRow
              key={day.date}
              day={day}
              index={index}
              onChange={updateDay}
              isCalendarDsr={day.isSunday || holidaySet.has(day.date)}
              isHoliday={holidaySet.has(day.date)}
            />
          ))}
        </CardContent>
      </Card>

      {result && (
        <Card>
          <CardContent>
            <Typography variant="h6">Processamento concluído</Typography>
            <Typography variant="body2">Colaborador: {result.employeeId}</Typography>
            <Typography variant="body2">
              Período: {(result.startDate ?? startDate)} até {(result.endDate ?? endDate)}
            </Typography>
            <Typography variant="body2" sx={{ mt: 1 }}>
              Abra a tela <strong>Resumo da Apuração</strong> para ver os cálculos detalhados por dia e totais.
            </Typography>
          </CardContent>
        </Card>
      )}

      <Box sx={{ mt: 2 }}>
        {message && <Alert severity="success">{message}</Alert>}
        {error && <Alert severity="error">{error}</Alert>}
        {!hasToken && <Alert severity="info">Faça login para habilitar operações.</Alert>}
      </Box>
    </Box>
  );
}
