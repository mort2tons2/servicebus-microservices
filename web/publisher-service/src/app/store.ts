import { configureStore } from "@reduxjs/toolkit";
import { setupListeners } from "@reduxjs/toolkit/query";
import { uploadsApi } from "../features/upload/uploadsApi";
import { uploadsReducer } from "../features/upload/uploadsSlice";

export const store = configureStore({
  reducer: {
    [uploadsApi.reducerPath]: uploadsApi.reducer,
    uploads: uploadsReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(uploadsApi.middleware),
});

setupListeners(store.dispatch);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
