import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { isAuthenticated } from '../utils/jwt';

const Header: React.FC = () => {
    const [authed, setAuthed] = useState<boolean>(isAuthenticated());

    useEffect(() => {
        const update = () => setAuthed(isAuthenticated());

        window.addEventListener('tokenChange', update);

        return () => {
            window.removeEventListener('tokenChange', update);
        };
    }, []);

    return (
        <header>
            <nav>
                <ul>
                    <li>
                        <Link to="/dashboard">Dashboard</Link>
                    </li>
                    {authed ? 
                    (<li><Link to="/logout">Logout</Link></li>) 
                    : (<li><Link to="/login">Login</Link></li>)}
                    {authed ? 
                    (<div></div>) 
                    : (<li><Link to="/register">Register</Link></li>)}
                    
                </ul>
            </nav>
        </header>
    );
};

export default Header;