import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import type { FileRow } from "../../components/FileTable";

export const thumbnailsApi = createApi({
  reducerPath: "thumbnailsApi",
  baseQuery: fetchBaseQuery({ baseUrl: "/api" }),
  endpoints: (build) => ({
    getThumbnails: build.query<FileRow[], void>({
      query: () => "/thumbnails",
      async onCacheEntryAdded(
        _arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved },
      ) {
        const connection = new HubConnectionBuilder()
          .withUrl("/hub/files")
          .withAutomaticReconnect()
          .build();

        connection.onclose((err) => {
          if (err) console.error("thumbnails hub closed", err);
        });

        try {
          await cacheDataLoaded;

          connection.on("fileProcessed", (row: FileRow) => {
            updateCachedData((draft) => {
              draft.unshift(row);
            });
          });

          connection.on("fileDeleted", ({ blobName }: { blobName: string }) => {
            updateCachedData((draft) => {
              const i = draft.findIndex((r) => r.blobName === blobName);
              if (i !== -1) draft.splice(i, 1);
            });
          });

          await connection.start();
        } catch (err) {
          console.error("thumbnails hub connection failed", err);
        }

        await cacheEntryRemoved;
        try {
          await connection.stop();
        } catch {
          // already disconnected
        }
      },
    }),
  }),
});

export const { useGetThumbnailsQuery } = thumbnailsApi;
