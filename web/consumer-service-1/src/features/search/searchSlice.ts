import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface SearchState {
  term: string;
}

const initialState: SearchState = { term: "" };

const searchSlice = createSlice({
  name: "search",
  initialState,
  reducers: {
    setTerm(state, action: PayloadAction<string>) {
      state.term = action.payload;
    },
  },
});

export const { setTerm } = searchSlice.actions;
export const searchReducer = searchSlice.reducer;
