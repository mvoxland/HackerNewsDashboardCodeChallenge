import React from 'react';
import { render, screen } from '@testing-library/react';
import App from '../App';

test('renders Hacker News Dashboard', () => {
  render(<App />);
  
  const linkElement = screen.getByText(/Hacker News Dashboard/i);
  expect(linkElement).toBeDefined();
});