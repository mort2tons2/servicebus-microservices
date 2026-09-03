import { AppBar, Container, Toolbar, Typography } from "@mui/material";
import { ThumbnailsList } from "./features/thumbnails/ThumbnailsList";

export function App() {
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="h1">
            Consumer service 2
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="md" sx={{ py: 4 }}>
        <Typography color="text.secondary" sx={{ mb: 3 }}>
          Frontend for <code>consumer-service2-api</code>. Shows the files this service
          has processed from its <code>consumer-service2</code> subscription.
        </Typography>

        <ThumbnailsList />
      </Container>
    </>
  );
}
