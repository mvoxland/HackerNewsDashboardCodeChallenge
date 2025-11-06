import React, { useEffect, useState } from 'react';
import { bestStoriesPreviewList } from '../services/apiService';
import StoryItem from './StoryItem';
import { HNStory } from '../types';

const StoryList: React.FC = () => {
    const [skip, setSkip] = useState<number>(0);
    const [take, setTake] = useState<number>(5);

    const [stories, setStories] = useState<HNStory[]>([]);
    const [count, setCount] = useState<number>();
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadStories = async () => {
            setLoading(true);
            setError(null);
            try {
                const fetchedStories = await bestStoriesPreviewList(skip, (count !== undefined && count - skip < take) ? count - skip : take);
                if (Array.isArray(fetchedStories.stories)) {
                    setStories(fetchedStories.stories);
                    setCount(fetchedStories.count);
                } else {
                    setStories([]);
                    setCount(0);
                    setError('Failed to parse stories');
                }
            } catch (err) {
                setError('Failed to fetch stories');
            } finally {
                setLoading(false);
            }
        };

        loadStories();
    }, [skip, take, count]);

    if (loading) {
        return <div>Loading stories...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    const totalPages = count ? Math.ceil(count / take) : undefined;
    const currentPage = Math.floor(skip / take) + 1;

    const goPrev = () => {
        setSkip(prev => Math.max(0, prev - take));
    };

    const goNext = () => {
        const maxSkip = Math.max(0, (totalPages! - 1) * take);
        setSkip(prev => Math.min(maxSkip, prev + take));
    };

    const changePageSize = (newTake: number) => {
        setTake(newTake);
        setSkip(0); // reset to first page
    };

    return (
        <div>
            <h2>
                Stories ({stories.length}/{count ?? '—'})
            </h2>

            <div className="pager">
                <button onClick={goPrev} disabled={count === undefined || skip <= 0}>
                    Prev
                </button>

                <span>
                    Page {totalPages ? `${currentPage} of ${totalPages}` : currentPage}
                </span>

                <button onClick={goNext} disabled={count !== undefined ? skip + take >= count : true}>
                    Next
                </button>

                <label className='pager-size-select'>
                    Page size:
                    <select
                        value={take}
                        onChange={(e) => changePageSize(Number(e.target.value))}
                    >
                        <option value={5}>5</option>
                        <option value={10}>10</option>
                        <option value={20}>20</option>
                        <option value={100}>100</option>
                    </select>
                </label>
            </div>

            <ul className="story-list">
                {stories.map(story => (
                    <StoryItem story={story} key={story.id} />
                ))}
            </ul>
        </div>
    );
};

export default StoryList;