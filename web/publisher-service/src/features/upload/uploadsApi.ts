import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

export interface UploadedFile {
  blobName: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  uploadTimestamp: string;
}

export const uploadsApi = createApi({
  reducerPath: "uploadsApi",
  baseQuery: fetchBaseQuery({ baseUrl: "/api" }),
  endpoints: (build) => ({
    uploadFile: build.mutation<UploadedFile, File>({
      query: (file) => {
        const body = new FormData();
        body.append("file", file);
        return { url: "/files", method: "POST", body };
      },
    }),
    deleteFile: build.mutation<void, string>({
      query: (blobName) => ({ url: `/files/${encodeURIComponent(blobName)}`, method: "DELETE" }),
    }),
  }),
});

export const { useUploadFileMutation, useDeleteFileMutation } = uploadsApi;
