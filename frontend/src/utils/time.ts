export function toTimeSpan(value: string): string | null {
  if (!value) {
    return null;
  }

  const normalized = value.length === 5 ? `${value}:00` : value;
  return normalized;
}

export function toDotnetTimeSpan(value: string): string | null {
  if (!value) {
    return null;
  }

  const [hoursPart, minutesPart] = value.split(":");
  const totalHours = Number(hoursPart);
  const minutes = Number(minutesPart ?? "0");

  if (Number.isNaN(totalHours) || Number.isNaN(minutes)) {
    return null;
  }

  const days = Math.floor(totalHours / 24);
  const hours = totalHours % 24;
  const hh = `${hours}`.padStart(2, "0");
  const mm = `${minutes}`.padStart(2, "0");

  return days > 0 ? `${days}.${hh}:${mm}:00` : `${hh}:${mm}:00`;
}

export function fromDotnetTimeSpan(value?: string | null): string {
  if (!value) {
    return "";
  }

  const isNegative = value.startsWith("-");
  const normalized = value.replace(",", ".").replace("-", "");
  const [dayPart, timePart] = normalized.includes(".")
    ? normalized.split(".", 2)
    : [null, normalized];

  const [hoursPart, minutesPart] = (timePart ?? "").split(":");
  const days = dayPart ? Number(dayPart) : 0;
  const hours = Number(hoursPart ?? "0");
  const minutes = Number(minutesPart ?? "0");

  if ([days, hours, minutes].some((item) => Number.isNaN(item))) {
    return "";
  }

  const totalHours = (days * 24) + hours;
  const hhmm = `${`${totalHours}`.padStart(2, "0")}:${`${minutes}`.padStart(2, "0")}`;
  return isNegative ? `-${hhmm}` : hhmm;
}

export function toDateInput(date: Date): string {
  const year = date.getFullYear();
  const month = `${date.getMonth() + 1}`.padStart(2, "0");
  const day = `${date.getDate()}`.padStart(2, "0");
  return `${year}-${month}-${day}`;
}
