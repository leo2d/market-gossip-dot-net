import moment from 'moment';

export const formatDateToLT = date => {
  return moment(date).format('LT');
};
