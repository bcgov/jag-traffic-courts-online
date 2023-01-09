export interface AuthState {
  isInitialized?: boolean;
  isLoading?: boolean;
  isAuthenticated?: boolean;
  userData?: any;
  accessToken?: string;
}

export const initialState: AuthState = {
  isInitialized: false,
  isLoading: false,
  isAuthenticated: false,
  userData: null,
  accessToken: null
}