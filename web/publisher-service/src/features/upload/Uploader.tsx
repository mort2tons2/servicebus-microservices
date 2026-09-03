import { type ChangeEvent } from "react";
import { Alert, Button, LinearProgress, Paper } from "@mui/material";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import { useUploadFileMutation } from "./uploadsApi";
import { fileUploaded } from "./uploadsSlice";
import { useAppDispatch } from "../../app/hooks";

function errorText(err: unknown): string {
  if (err && typeof err === "object" && "status" in err) {
    const e = err as { status: unknown; data?: unknown };
    if (typeof e.data === "string" && e.data) return e.data;
    return `Request failed (${String(e.status)})`;
  }
  return "Upload failed";
}

export function Uploader() {
  const dispatch = useAppDispatch();
  const [uploadFile, { isLoading, error }] = useUploadFileMutation();

  async function onPick(e: ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    e.target.value = "";

    if (!file) return;

    try {
      const result = await uploadFile(file).unwrap();
      dispatch(fileUploaded(result));
    } catch {
      /* empty */
    }
  }

  return (
    <Paper variant="outlined" sx={{ p: 2 }}>
      <Button
        component="label"
        variant="contained"
        startIcon={<UploadFileIcon />}
        disabled={isLoading}
      >
        Choose file &amp; upload
        <input type="file" hidden onChange={onPick} />
      </Button>
      {isLoading && <LinearProgress sx={{ mt: 2 }} />}
      {error && (
        <Alert severity="error" sx={{ mt: 2 }}>
          {errorText(error)}
        </Alert>
      )}
    </Paper>
  );
}
