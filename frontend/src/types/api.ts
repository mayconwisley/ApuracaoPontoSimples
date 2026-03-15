export type HolidayType = 0 | 1 | 2;
export type DayCode = 0 | 1 | 2 | 3 | 4 | 5 | 6;
export type AbsenceType = 0 | 1 | 2;

export interface AuthResponse {
  token: string;
}

export interface Employee {
  id: string;
  name: string;
  pis?: string | null;
  admissionDate?: string | null;
  employerId: string;
}

export interface ScheduleConfig {
  dailyHours: string;
  dailyLimit?: string | null;
  saturdayHours?: string | null;
  toleranceEntry?: string | null;
  toleranceExit?: string | null;
  nightStart?: string | null;
  nightEnd?: string | null;
  weeklyHours?: string | null;
  saturdayCountsAsBank: boolean;
  useDailyHoursAsY13: boolean;
}

export interface EmployeeDetails extends Employee {
  schedule?: ScheduleConfig | null;
}

export interface Employer {
  id: string;
  name: string;
  cnpj?: string | null;
  address?: string | null;
}

export interface Holiday {
  id: string;
  date: string;
  description: string;
}

export interface TimeIntervalResult {
  start?: string | null;
  end?: string | null;
}

export interface AbsenceResult {
  type?: number;
  hours?: string | null;
}

export interface DayCalculationResult {
  apuracao?: string | null;
  bancoHoras?: string | null;
  horasPositivas?: string | null;
  horasNegativas?: string | null;
  horasExtras?: string | null;
  horasDsrFeriado?: string | null;
  horasNoturnas?: string | null;
}

export interface TimeCardDayResult {
  id: string;
  date: string;
  code?: number;
  holidayType?: number;
  isSunday?: boolean;
  isSaturday?: boolean;
  interval1: TimeIntervalResult;
  interval2: TimeIntervalResult;
  interval3: TimeIntervalResult;
  absence?: AbsenceResult | null;
  calculation?: DayCalculationResult | null;
}

export interface TimeCardTotals {
  bancoHorasTotal: string;
  apuracaoTotal: string;
  horasPositivasTotal: string;
  horasNegativasTotal: string;
  horasNoturnasTotal: string;
  horasExtrasTotal: string;
  horasDsrFeriadoTotal: string;
}

export interface TimeCard {
  id: string;
  employeeId: string;
  startDate?: string | null;
  endDate?: string | null;
  days?: TimeCardDayResult[];
  totals?: TimeCardTotals | null;
}
