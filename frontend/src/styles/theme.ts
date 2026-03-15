import { createTheme } from "@mui/material/styles";

export const theme = createTheme({
  palette: {
    mode: "light",
    primary: {
      main: "#0C2E4E",
      contrastText: "#F7F4EE",
    },
    secondary: {
      main: "#F2B705",
    },
    background: {
      default: "#F7F4EE",
      paper: "#FFFFFF",
    },
  },
  typography: {
    fontFamily: "'Plus Jakarta Sans', 'Segoe UI', sans-serif",
  },
});
