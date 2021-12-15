//
// Created by Urban Vidovič on 14/12/2021.
//

#ifndef PARTICLE_SYSTEM_MAINWINDOW_H
#define PARTICLE_SYSTEM_MAINWINDOW_H

#include <QMainWindow>

namespace Ui {
    class MainWindow;
}

class MainWindow : public QMainWindow{
Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow() override;

private slots:
    void on_actionTest_triggered();

    void on_pushButtonRotacija_clicked();

    void on_pushButtonZelenaBarva_clicked();

private:
    Ui::MainWindow *ui;
};

#endif //PARTICLE_SYSTEM_MAINWINDOW_H
