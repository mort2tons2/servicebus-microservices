import { createTheme } from "@mui/material/styles";

export const theme = createTheme({
  colorSchemes: { light: true, dark: true },
  cssVariables: { colorSchemeSelector: "media" },
  palette: { primary: { main: "#2563eb" } },
  shape: { borderRadius: 8 },
});
