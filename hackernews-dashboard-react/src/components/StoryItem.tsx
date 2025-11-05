import React from 'react';
import { HNStory } from '../types';

interface StoryItemProps {
    story: HNStory;
}

const StoryItem: React.FC<StoryItemProps> = ( {story} ) => {
    return (
        <div className="story-item">
            <h3>{story.title}</h3>
            <p>By: {story.by}</p>
            <p>Points: {story.score}</p>
            <a href={story.url} target="_blank" rel="noopener noreferrer">Read more</a>
        </div>
    );
};

export default StoryItem;