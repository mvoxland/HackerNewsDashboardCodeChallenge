import axios from 'axios';
import { HNStory } from '../types';

const API_BASE_URL = 'http://localhost/api';

export const bestStoriesPreviewList = async (skip: number, take: number) => {
    try {
        const response = await axios.get<HNStory[]>(`${API_BASE_URL}/bestStoriesPreviewList?skip=${skip}&take=${take}`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching top stories');
    }
};