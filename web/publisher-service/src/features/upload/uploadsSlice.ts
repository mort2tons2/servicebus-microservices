import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { UploadedFile } from "./uploadsApi";

interface UploadsState {
  recent: UploadedFile[];
}

const initialState: UploadsState = { recent: [] };

const uploadsSlice = createSlice({
  name: "uploads",
  initialState,
  reducers: {
    fileUploaded(state, action: PayloadAction<UploadedFile>) {
      state.recent.unshift(action.payload);
    },
    fileDeleted(state, action: PayloadAction<{ blobName: string }>) {
      state.recent = state.recent.filter((f) => f.blobName !== action.payload.blobName);
    },
  },
});

export const { fileUploaded, fileDeleted } = uploadsSlice.actions;
export const uploadsReducer = uploadsSlice.reducer;
