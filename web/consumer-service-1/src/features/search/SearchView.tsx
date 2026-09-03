import { Alert, TextField } from "@mui/material";
import { useGetDocumentsQuery } from "./searchApi";
import { setTerm } from "./searchSlice";
import { useAppDispatch, useAppSelector } from "../../app/hooks";
import { FileTable } from "../../components/FileTable";

export function SearchView() {
  const term = useAppSelector((s) => s.search.term);
  const dispatch = useAppDispatch();

  const { data = [], error } = useGetDocumentsQuery(term);

  return (
    <>
      <TextField
        fullWidth
        size="small"
        type="search"
        placeholder="filter by file name…"
        value={term}
        onChange={(e) => dispatch(setTerm(e.target.value))}
        sx={{ mb: 2 }}
      />
      {error ? (
        <Alert severity="error">Could not reach search-api.</Alert>
      ) : (
        <FileTable rows={data} />
      )}
    </>
  );
}
