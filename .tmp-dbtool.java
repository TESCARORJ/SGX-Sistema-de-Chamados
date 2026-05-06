import java.sql.*;

public class TmpDbTool {
  public static void main(String[] args) throws Exception {
    if (args.length == 0) {
      throw new IllegalArgumentException("Informe SQL");
    }
    String sql = args[0];
    String url = "jdbc:postgresql://localhost:5432/chamados_geti";
    String user = "chamados_geti_user";
    String pass = "chamadosgeti@001";
    try (Connection c = DriverManager.getConnection(url, user, pass);
         Statement s = c.createStatement()) {
      boolean hasResult = s.execute(sql);
      if (hasResult) {
        try (ResultSet rs = s.getResultSet()) {
          ResultSetMetaData md = rs.getMetaData();
          int cols = md.getColumnCount();
          while (rs.next()) {
            StringBuilder b = new StringBuilder();
            for (int i = 1; i <= cols; i++) {
              if (i > 1) b.append(" | ");
              b.append(md.getColumnLabel(i)).append("=").append(rs.getString(i));
            }
            System.out.println(b);
          }
        }
      } else {
        System.out.println("UPDATED=" + s.getUpdateCount());
      }
    }
  }
}
