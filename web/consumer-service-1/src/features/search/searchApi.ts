import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import type { FileRow } from "../../components/FileTable";

export const searchApi = createApi({
  reducerPath: "searchApi",
  baseQuery: fetchBaseQuery({ baseUrl: "/api" }),
  endpoints: (build) => ({
    getDocuments: build.query<FileRow[], string>({
      query: (term) => {
        const t = term.trim();
        return t ? `/search?q=${encodeURIComponent(t)}` : "/documents";
      },
      async onCacheEntryAdded(
        term,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved },
      ) {
        if (term.trim()) return;

        const connection = new HubConnectionBuilder()
          .withUrl("/hub/files")
          .withAutomaticReconnect()
          .build();

        connection.onclose((err) => {
          if (err) console.error("search hub closed", err);
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
          console.error("search hub connection failed", err);
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

export const { useGetDocumentsQuery } = searchApi;
