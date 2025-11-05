import React from 'react';
import StoryList from '../components/StoryList';

const Dashboard: React.FC = () => {
    return (
        <div>
            <h1>Welcome to the Hacker News Dashboard!</h1>
            <StoryList />
        </div>
    );
};

export default Dashboard;