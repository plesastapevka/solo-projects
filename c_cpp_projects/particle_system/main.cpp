#include <QCoreApplication>
#include <QDebug>
#include <GLFW/glfw3.h>

auto main(int argc, char *argv[]) -> int {
    QCoreApplication a(argc, argv);
    qDebug() << "Hello World";
    return QCoreApplication::exec();
}