import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { removeTokens } from '../utils/jwt';

const Logout = () => {
    const [error, setError] = useState<string>('');
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            removeTokens();

            navigate('/login');
        } catch (err) {
            setError('Registration failed. Please try again.');
        }
    };

    return (
        <div>
            <h2>Register</h2>
            {error && <p style={{ color: 'red' }}>{error}</p>}
            <form onSubmit={handleSubmit}>
                Are you sure you wish to logout?
                <button type="submit">Logout</button>
            </form>
        </div>
    );
};

export default Logout;