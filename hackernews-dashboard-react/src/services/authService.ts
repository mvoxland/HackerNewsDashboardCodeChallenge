import axios from 'axios';
import { Token } from '../types';
import { setRefreshToken, setToken } from '../utils/jwt';

const API_URL = 'https://localhost/api';

export const login = async (email: string, password: string) => {
    const response = await axios.post<Token>(`${API_URL}/login`, { email: email, password: password });
    if (response.data.accessToken) {
        setToken(response.data.accessToken);
        setRefreshToken(response.data.refreshToken);

        // todo - notify auth state change
    }
    return;
};

export const register = async (email: string, password: string, userName: string) => {
    await axios.post(`${API_URL}/register`, { email, password, userName });
    
    //todo - make this flow? await login( email, password );

    return;
};