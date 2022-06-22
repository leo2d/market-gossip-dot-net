export const isCommand = message => {
  const trimmedStr = message.trim();
  return trimmedStr.startsWith('/') && trimmedStr.includes('=');
};
