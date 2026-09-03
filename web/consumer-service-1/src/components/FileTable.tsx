import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";

export interface FileRow {
  blobName: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  uploadTimestamp: string;
}

function formatBytes(n: number): string {
  if (n < 1024) return `${n} B`;
  if (n < 1024 * 1024) return `${(n / 1024).toFixed(1)} KB`;
  return `${(n / (1024 * 1024)).toFixed(1)} MB`;
}

export function FileTable({ rows }: { rows: FileRow[] }) {
  if (rows.length === 0) {
    return (
      <Typography color="text.secondary" variant="body2">
        Nothing yet.
      </Typography>
    );
  }

  return (
    <Table size="small">
      <TableHead>
        <TableRow>
          <TableCell>File</TableCell>
          <TableCell>Type</TableCell>
          <TableCell align="right">Size</TableCell>
          <TableCell align="right">Uploaded</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {rows.map((f) => (
          <TableRow key={f.blobName}>
            <TableCell>{f.fileName}</TableCell>
            <TableCell>{f.contentType}</TableCell>
            <TableCell align="right">{formatBytes(f.sizeBytes)}</TableCell>
            <TableCell align="right">
                    {new Date(f.uploadTimestamp).toLocaleTimeString()}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}
