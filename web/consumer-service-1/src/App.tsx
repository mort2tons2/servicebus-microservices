import { AppBar, Container, Toolbar, Typography } from "@mui/material";
import { SearchView } from "./features/search/SearchView";

export function App() {
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="h1">
            Consumer service 1
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="md" sx={{ py: 4 }}>
        <Typography color="text.secondary" sx={{ mb: 3 }}>
          Frontend for <code>consumer-service1-api</code>. Filter in files that this service has
          processed from its <code>consumer-service1</code> subscription.
        </Typography>

        <SearchView />
      </Container>
    </>
  );
}
