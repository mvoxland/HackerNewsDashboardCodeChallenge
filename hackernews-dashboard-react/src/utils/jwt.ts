import { jwtDecode, JwtPayload } from 'jwt-decode';

export const getToken = (): string | null => {
    return localStorage.getItem('token');
};

export const getRefreshToken = (): string | null => {
    return localStorage.getItem('refreshToken');
};

export const setToken = (token: string): void => {
    localStorage.setItem('token', token);
    window.dispatchEvent(new Event('tokenChange'));
};

export const setRefreshToken = (refreshToken: string): void => {
    localStorage.setItem('refreshToken', refreshToken);
    window.dispatchEvent(new Event('tokenChange'));
}

export const removeTokens = (): void => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    window.dispatchEvent(new Event('tokenChange'));
};

export const decodeToken = (token: string): JwtPayload => {
    return jwtDecode(token) as JwtPayload;
};

export const isTokenExpired = (token: string): boolean => {
    const decoded = decodeToken(token);
    return (decoded.exp || 0) * 1000 < Date.now();
};

export const isAuthenticated = (): boolean => {
    const token = getToken();
    return token !== null && !isTokenExpired(token);
};

// todo - add token refresh