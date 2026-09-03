import { Alert, Typography } from "@mui/material";
import { FileTable } from "../../components/FileTable";
import { useAppDispatch, useAppSelector } from "../../app/hooks";
import { useDeleteFileMutation } from "./uploadsApi";
import { fileDeleted } from "./uploadsSlice";

export function RecentUploads() {
  const dispatch = useAppDispatch();
  const recent = useAppSelector((s) => s.uploads.recent);
  const [deleteFile, { isLoading, originalArgs, error }] = useDeleteFileMutation();

  async function onDelete(blobName: string) {
    try {
      await deleteFile(blobName).unwrap();
      dispatch(fileDeleted({ blobName }));
    } catch {
      /* empty */
    }
  }

  return (
    <>
      <Typography variant="h6" sx={{ mb: 1.5 }}>
        Uploaded this session
      </Typography>
      {error && (
        <Alert severity="error" sx={{ mb: 1.5 }}>
          Delete failed
        </Alert>
      )}
      <FileTable
        rows={recent}
        onDelete={onDelete}
        deletingBlobName={isLoading ? originalArgs : undefined}
      />
    </>
  );
}
