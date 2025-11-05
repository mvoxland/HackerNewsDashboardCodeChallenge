export interface Token {
    accessToken: string;
    refreshToken: string;
}

export interface StoriesWithCount {
    stories: HNStory[];
    count: number;
}

export interface HNStory {
    id: string;
    by: string;
    time?: number;
    kids?: number[];
    url?: string;
    score?: number;
    title?: string;
    descendants?: number;
    userComments?: HNComment[];
    userRating?: UserRating
}

export interface HNComment {
    id: string;
    by: string;
    time?: number;
    text?: string;
    parent?: string;
    kids?: number[];
}

export interface UserRating {
    itemId: number;
    username: string;
    ratingStars: number;
    ratingDateTime: string;
}

// export interface User {
//     id: string;
//     username: string;
//     email: string;
//     createdAt: string;
// }

// export interface Story {
//     id: string;
//     title: string;
//     url: string;
//     score: number;
//     by: string;
//     time: number;
//     descendants: number;
// }

// export interface Comment {
//     id: string;
//     by: string;
//     text: string;
//     time: number;
//     parent: string;
//     kids?: string[];
// }

// export interface AuthResponse {
//     token: string;
//     user: User;
// }

// export interface RegisterData {
//     username: string;
//     email: string;
//     password: string;
// }

// export interface LoginData {
//     username: string;
//     password: string;
// }