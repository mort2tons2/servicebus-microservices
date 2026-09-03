import {
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";

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

interface FileTableProps {
  rows: FileRow[];
  onDelete?: (blobName: string) => void;
  deletingBlobName?: string;
}

export function FileTable({ rows, onDelete, deletingBlobName }: FileTableProps) {
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
          {onDelete && <TableCell align="right" />}
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
            {onDelete && (
              <TableCell align="right">
                <IconButton
                  size="small"
                  aria-label={`Delete ${f.fileName}`}
                  disabled={deletingBlobName === f.blobName}
                  onClick={() => onDelete(f.blobName)}
                >
                  <DeleteIcon fontSize="small" />
                </IconButton>
              </TableCell>
            )}
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}
