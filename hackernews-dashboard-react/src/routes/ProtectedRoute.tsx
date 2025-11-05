import { PropsWithChildren } from 'react';
import { Navigate } from 'react-router-dom';
import { isAuthenticated } from '../utils/jwt';

const ProtectedRoute = ({ children }: PropsWithChildren<{}>) => {
    return isAuthenticated() ? <>{children}</> : <Navigate to="/login" replace />;
};

export default ProtectedRoute;