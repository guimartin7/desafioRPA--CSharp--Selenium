CREATE TABLE [dbo].[resultadoRPA] (
    [COD]          INT          IDENTITY (1, 1) NOT NULL,
    [wpm]          NUMERIC NULL,
    [keystrokes]   NUMERIC NULL,
    [accuracy]     NVARCHAR(50) NULL,
    [correctWords] NUMERIC NULL,
    [wrongWords]   NUMERIC NULL
);

