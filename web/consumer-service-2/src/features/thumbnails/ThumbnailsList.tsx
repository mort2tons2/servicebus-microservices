import { Alert } from "@mui/material";
import { useGetThumbnailsQuery } from "./thumbnailsApi";
import { FileTable } from "../../components/FileTable";

export function ThumbnailsList() {
  const { data = [], error } = useGetThumbnailsQuery(undefined);

  if (error) {
    return <Alert severity="error">Could not reach thumbnails-api.</Alert>;
  }

  return <FileTable rows={data} />;
}
