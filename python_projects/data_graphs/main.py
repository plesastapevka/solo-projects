import pandas as pd
import plotly.express as px
import plotly.graph_objects as go

DATA_PATH = "data/DebelinaRazreza.csv"


def plot_graph():
    df = pd.read_csv("data/DebelinaRazreza.csv")
    fig = px.line(df, x="Date.Time", y=df.columns[1:-1])
    data_info = df.describe()
    print(data_info)
    minimum = data_info.min()
    first_q = data_info.quantile(q=0.25)
    median = data_info.median()
    third_q = data_info.quantile(q=0.75)
    maximum = data_info.max()
    table = go.Figure(data=[
        go.Table(header=dict(values=["", "Pos1", "Pos2", "Pos3", "Pos4", "Pos5", "Pos6"]),
                 cells=dict(values=[["minimum", "1. kvartil", "2. kvartil/mediana", "3. kvartil", "maksimum"],
                                    [minimum[0], first_q[0], median[0], third_q[0], maximum[0]],
                                    [minimum[1], first_q[1], median[1], third_q[1], maximum[1]],
                                    [minimum[2], first_q[2], median[2], third_q[2], maximum[2]],
                                    [minimum[3], first_q[3], median[3], third_q[3], maximum[3]],
                                    [minimum[4], first_q[4], median[4], third_q[4], maximum[4]],
                                    [minimum[5], first_q[5], median[5], third_q[5], maximum[5]],
                                    ]))])

    with open("index.html", "w") as f:
        f.write(fig.to_html(full_html=False, include_plotlyjs='cdn'))
        f.write(table.to_html(full_html=False, include_plotlyjs='cdn'))
        f.write("<div><p>Vidimo, da gre za odstopanja predvsem pri poziciji 1</p></div>")


def main():
    plot_graph()


if __name__ == '__main__':
    main()
