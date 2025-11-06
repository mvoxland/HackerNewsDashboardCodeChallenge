import React from 'react';
import { HNStory } from '../types';

interface StoryItemProps {
    story: HNStory;
}

const StoryItem: React.FC<StoryItemProps> = ( {story} ) => {
    return (
        <div className="story-item">
            <h3>{story.title}</h3>
            <a href={story.url} target="_blank" rel="noopener noreferrer" className='wrap-text'>{story.url}</a>
            <p>By: {story.by}</p>
            <p>Score: {story.score}</p>
            <p>Comments: {story.kids?.length} ({story.descendants})</p>
            <p>Posted: {story.time ? new Date(story.time * 1000).toLocaleString(undefined, { dateStyle: 'short', timeStyle: 'short'}) : 'N/A'}</p>

            <hr />

            <p>User Comments: {story.userComments?.length}</p>
            <p>Your Rating: {story.userRating?.ratingStars ? story.userRating?.ratingStars + " stars" : ""}</p>
        </div>
    );
};

export default StoryItem;