import { AppBar, Box, Container, Toolbar, Typography } from "@mui/material";
import { Uploader } from "./features/upload/Uploader";
import { RecentUploads } from "./features/upload/RecentUploads";

export function App() {
  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="h1">
            Uploads to blob (Publisher service)
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="md" sx={{ py: 4 }}>
        <Typography color="text.secondary" sx={{ mb: 3 }}>
          Frontend for <code>publisher-service-api</code>. Sends a file to blob storage and
          publishes a <code>FileUploadedEvent</code> or <code>FileDeletedEvent</code>
        </Typography>

        <Uploader />

        <Box sx={{ mt: 3 }}>
          <RecentUploads />
        </Box>
      </Container>
    </>
  );
}
