import { configureStore } from "@reduxjs/toolkit";
import { setupListeners } from "@reduxjs/toolkit/query";
import { thumbnailsApi } from "../features/thumbnails/thumbnailsApi";

export const store = configureStore({
  reducer: {
    [thumbnailsApi.reducerPath]: thumbnailsApi.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(thumbnailsApi.middleware),
});

setupListeners(store.dispatch);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
