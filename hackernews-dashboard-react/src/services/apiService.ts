import axios from 'axios';
import { StoriesWithCount } from '../types';
import { getToken } from '../utils/jwt';

const API_BASE_URL = 'http://localhost/api';

export const bestStoriesPreviewList = async (skip: number, take: number) => {
    try {
        const headers = getToken() ? { Authorization: `Bearer ${getToken()}` } : undefined;
        const response = await axios.get<StoriesWithCount>(`${API_BASE_URL}/bestStoriesPreviewList?skip=${skip}&take=${take}`, 
            { headers });
        return response.data;
    } catch (error) {
        throw new Error('Error fetching top stories');
    }
};