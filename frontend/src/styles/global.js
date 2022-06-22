import { createGlobalStyle } from 'styled-components';

const GlobalStyle = createGlobalStyle`
    *{
        margin:0;
        padding: 0;
        outline: 0;
        box-sizing: border-box;
    }

    #root{
        margin: 0 auto;
        
        /* padding: 0 20px 50px;
        max-width: 1020px; */
    }

    button{
        cursor: pointer;
    }

    body{
        background-color: #383f9f;
    }
`;

export default GlobalStyle;
