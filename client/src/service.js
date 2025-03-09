import axios from 'axios';

const apiUrl = "http://localhost:5161";
axios.defaults.baseURL = apiUrl;

// הוספת interceptor עבור שגיאות ב-response
axios.interceptors.response.use(
  response => response, // אם הבקשה הצליחה, מחזירים את התגובה
  error => {
    console.error("Error response:", error.response); // רישום השגיאה ללוג
    return Promise.reject(error); // מחזירים את השגיאה כדי שניתן יהיה לטפל בה מאוחר יותר
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`/`);    
    return result.data;
  },

  addTask: async(name) => {
    try {
      const response = await axios.post(`/items`, { name }); // משתמשים ב-endpoint /items
      return response.data; // מחזירים את הנתונים שהתקבלו בתגובה
    } catch (error) {
      console.error("Error adding task:", error.response); // רישום השגיאה ללוג
      throw error; // זורקים את השגיאה כדי שניתן יהיה לטפל בה במעלה הזרימה
    }
  },
  

  setCompleted: async(id, isComplete)=>{
   
    await axios.put(`${apiUrl}/updateItem?id=${id}&&IsComplete=${isComplete}`);
  },

  deleteTask: async (id) => {
    try {
      await axios.delete(`/items/${id}`); // משתמשים ב-endpoint /items/{id}
      console.log(`Task with id ${id} deleted successfully`); // רישום הצלחה ללוג
    } catch (error) {
      console.error("Error deleting task:", error.response); // רישום השגיאה ללוג
      throw error; // זורקים את השגיאה כדי שניתן יהיה לטפל בה במעלה הזרימה
    }
  },
};