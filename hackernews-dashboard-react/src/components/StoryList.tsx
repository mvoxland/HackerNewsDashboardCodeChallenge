import React, { useEffect, useState } from 'react';
import { bestStoriesPreviewList } from '../services/apiService';
import StoryItem from './StoryItem';
import { HNStory } from '../types';

const StoryList: React.FC = () => {
    const [stories, setStories] = useState<HNStory[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadStories = async () => {
            try {
                const fetchedStories = await bestStoriesPreviewList(0, 6);
                setStories(fetchedStories);
            } catch (err) {
                setError('Failed to fetch stories');
            } finally {
                setLoading(false);
            }
        };

        loadStories();
    }, []);

    if (loading) {
        return <div>Loading stories...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    return (
        <div>
            <h2>Stories</h2>
            <ul>
                {stories.map(story => (
                    <StoryItem story={story} />
                ))}
            </ul>
        </div>
    );
};

export default StoryList;