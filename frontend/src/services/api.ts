import type { AuthResponse, Employee, EmployeeDetails, Employer, Holiday, TimeCard } from "../types/api";

const API_BASE = import.meta.env.VITE_API_URL ?? "http://localhost:5024/api";

type HttpMethod = "GET" | "POST" | "PUT" | "DELETE";

async function request<T>(
  path: string,
  method: HttpMethod,
  body?: unknown,
  token?: string
): Promise<T> {
  const response = await fetch(`${API_BASE}${path}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || "Erro na chamada da API.");
  }

  if (response.status === 204) {
    return {} as T;
  }

  return (await response.json()) as T;
}

export const authApi = {
  bootstrap: (payload: { email: string; password: string; fullName: string }) =>
    request<AuthResponse>("/auth/bootstrap", "POST", payload),
  login: (payload: { email: string; password: string }) =>
    request<AuthResponse>("/auth/login", "POST", payload),
  register: (
    payload: { email: string; password: string; fullName: string; role: string },
    token: string
  ) => request<AuthResponse>("/auth/register", "POST", payload, token),
};

export const employeeApi = {
  list: (token: string) => request<Employee[]>("/employees", "GET", undefined, token),
  getById: (id: string, token: string) => request<EmployeeDetails>(`/employees/${id}`, "GET", undefined, token),
  create: (payload: unknown, token: string) =>
    request<Employee>("/employees", "POST", payload, token),
  update: (id: string, payload: unknown, token: string) =>
    request<Employee>(`/employees/${id}`, "PUT", payload, token),
  remove: (id: string, token: string) =>
    request<void>(`/employees/${id}`, "DELETE", undefined, token),
};

export const employerApi = {
  list: (token: string) => request<Employer[]>("/employers", "GET", undefined, token),
  create: (payload: { name: string; cnpj?: string | null; address?: string | null }, token: string) =>
    request<Employer>("/employers", "POST", payload, token),
  update: (id: string, payload: { name: string; cnpj?: string | null; address?: string | null }, token: string) =>
    request<Employer>(`/employers/${id}`, "PUT", payload, token),
  remove: (id: string, token: string) =>
    request<void>(`/employers/${id}`, "DELETE", undefined, token),
};

export const holidayApi = {
  list: (token: string) => request<Holiday[]>("/holidays", "GET", undefined, token),
  create: (payload: { date: string; description: string }, token: string) =>
    request<Holiday>("/holidays", "POST", payload, token),
  update: (id: string, payload: { date: string; description: string }, token: string) =>
    request<Holiday>(`/holidays/${id}`, "PUT", payload, token),
  remove: (id: string, token: string) =>
    request<void>(`/holidays/${id}`, "DELETE", undefined, token),
};

export const timeCardApi = {
  create: (payload: unknown, token: string) =>
    request<TimeCard>("/timecards", "POST", payload, token),
  get: (employeeId: string, startDate: string, endDate: string, token: string) =>
    request<TimeCard>(`/timecards/${employeeId}/${startDate}/${endDate}`, "GET", undefined, token),
};
