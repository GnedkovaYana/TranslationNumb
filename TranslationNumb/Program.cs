using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TranslatNumb
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                string textF = "~Перевод целых и вещественных чисел для пятиклассника~";
                string textT = "Выполнила: Гнедкова Яна, группа: ПрИ - 102";
                int CenterF = (Console.WindowWidth / 2) - (textF.Length / 2);
                Console.SetCursorPosition(CenterF, 0);
                Console.WriteLine(textF);
                int topRightX = Console.WindowWidth - textT.Length;
                Console.SetCursorPosition(topRightX, 2);
                Console.WriteLine(textT);
                Console.WriteLine();
                Console.WriteLine("Выберите, какую из функций будете использовать:");
                Console.WriteLine();
                Console.WriteLine("Функция 1: Перевод целых чисел в дополнительный код");
                Console.WriteLine("Функция 2: Сложение целых (положительных и отрицательных) чисел с использованием дополнительного кода");
                Console.WriteLine("Функция 3: Процесс перевода вещественных чисел в формат с плавающей точкой");
                Console.WriteLine("Функция 4: Процесс сложения вещественных чисел в формат с плавающей точкой");
                Console.WriteLine();
                string operation = Console.ReadLine().Trim();
                Console.WriteLine();

                try
                {
                    if (operation == "1")
                    {
                        ConverterToAdditionalStart();
                    }
                    else if (operation == "2")
                    {
                        AdditionalSumStart();
                    }
                    else if (operation == "3")
                    {
                        ConverterFromFloatToBinaryFloatStart();
                    }
                    else if (operation == "4")
                    {
                        SumFloatBinaryStart();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Функция введена неправильно");
                        Console.ResetColor();
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine();
                    Console.Write("Ошибка:");
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.WriteLine("Если хотите продолжить работу с программой, нажмите любую кнопку");
                string operationEnd = Console.ReadLine();
                Console.Clear();
            }
        }
        private static void SumFloatBinaryStart()
        {
            Console.Write("Введите первое число: ");
            string number1Input = Console.ReadLine();
            if (!double.TryParse(number1Input, out double number1))
                throw new ArgumentException();
            Console.Write("Введите второе число: ");
            string number2Input = Console.ReadLine();
            if (!double.TryParse(number2Input, out double number2))
                throw new ArgumentException();

            Console.WriteLine();
            Console.WriteLine("Для начала определим знак итогового ответа");
            Console.WriteLine();

            double number1Abs = Math.Abs(number1);
            double number2Abs = Math.Abs(number2);
            double maxNumberAbs = Math.Max(number1Abs, number2Abs);

            int resultSign;

            if (maxNumberAbs == number1Abs)
            {
                Console.WriteLine("Так как число {0} по модулю больше числа {1}, то знак итогового числа совпадает со знаком числа {0}", number1, number2);
                resultSign = Math.Sign(number1);
            }
            else
            {
                Console.WriteLine("Так как число {0} по модулю больше числа {1}, то знак итогового числа совпадает со знаком числа {0}", number2, number1);
                resultSign = Math.Sign(number2);
            }

            if (resultSign == -1)
            {
                resultSign = 1;
            }
            else
            {
                resultSign = 0;
            }

            Console.WriteLine("Следовательно, знак результата сложения в формате нормализованной записи: {0}", resultSign);

            Console.WriteLine();
            Console.WriteLine("Перед тем как производить сложение, нужно перевести оба числа в формат нормализованной записи:");

            string binaryFloatNumber1 = ConverterFromFloatToBinaryFloat(number1);
            string binaryFloatNumber2 = ConverterFromFloatToBinaryFloat(number2);

            Console.WriteLine("Теперь производим суммирование двух чисел в формате нормализованной записи:");
            Console.WriteLine();

            Console.WriteLine("Первым делом нам нужно добиться равенства смещенного порядка у двух чисел:");
            Console.WriteLine();

            string numberSign1 = binaryFloatNumber1.Substring(0, 1);
            string numberOrder1 = binaryFloatNumber1.Substring(2, 8);
            string mantissa1 = binaryFloatNumber1.Substring(11);

            string numberSign2 = binaryFloatNumber2.Substring(0, 1);
            string numberOrder2 = binaryFloatNumber2.Substring(2, 8);
            string mantissa2 = binaryFloatNumber2.Substring(11);

            if (FromAnyToDec(numberOrder1, 2) == FromAnyToDec(numberOrder2, 2))
            {
                Console.WriteLine("Т.к. у чисел порядки равны, то делать ничего не нужно");
            }

            else if (FromAnyToDec(numberOrder1, 2) > FromAnyToDec(numberOrder2, 2))
            {
                Alignment(binaryFloatNumber1, ref binaryFloatNumber2, numberOrder1, numberSign2, ref numberOrder2, ref mantissa2);
            }
            else
            {
                Alignment(binaryFloatNumber2, ref binaryFloatNumber1, numberOrder2, numberSign1, ref numberOrder1, ref mantissa1);
            }
            Console.WriteLine();
            Console.WriteLine("Числа после выравнивания порядка:");
            Console.WriteLine("Первое число: {0}", binaryFloatNumber1);
            Console.WriteLine("Второе число: {0}", binaryFloatNumber2);
            Console.WriteLine();
            Console.WriteLine("Теперь осталось только сложить мантиссы и подставить полученный в самом начале знак итогового числа:");
            string resultOrder = numberOrder2;
            string resultMantissa = CorrectSum(mantissa1, mantissa2);

            if (resultMantissa.Length > 23)
            {
                Console.WriteLine();
                Console.WriteLine("Т.к. итоговое значение мантиссы имеет длину больше 23, то нам нужно");
                Console.WriteLine("Увеличить значение смещённого порядка на 1, \"отрезать\" от мантиссы первый элемент и сдвинуть её вправо");
                resultOrder = Sum(resultOrder, "1");
                resultMantissa = resultMantissa.Substring(1);
                resultMantissa = "0" + resultMantissa;
                resultMantissa = resultMantissa.Substring(0, 23);
            }

            Console.WriteLine();

            string resultBinary = resultSign + "|" + resultOrder + "|" + resultMantissa;

            float resultDec = (float)number1 + (float)number2;

            Console.WriteLine("Итоговый ответ в формате нормализованной записи: {0}", resultBinary);
            Console.WriteLine("Итоговый ответ в десятичной системе счисления: {0}", resultDec);
            Console.ResetColor();

        }

        private static void Alignment(string binaryFloatNumber1, ref string binaryFloatNumber2, string numberOrder1, string numberSign2, ref string numberOrder2, ref string mantissa2)
        {
            Console.WriteLine("Так как у числа {0} порядок больше,\n" +
                                "то нам нужно увеличивать порядок числа {1}", binaryFloatNumber1, binaryFloatNumber2);
            Console.WriteLine();
            Console.WriteLine("Для этого будем сдвигать мантиссу вправо и увеличивать значение смещенного порядка на 1, пока порядки не сравняются: ");
            //Console.WriteLine("(Примечание: после первого сдвига мантиссы, она будет начинаться не с 0, а с 1, т.к. эта 1 осталась между \n" +
                //"смещенным порядком и мантиссой после представления числа в нормализованной экспоненциальной форме,\n" +
                //"просто мы её не записывали для экономии памяти)");
            Console.WriteLine();
            bool flag = true;
            while (numberOrder2 != numberOrder1)
            {
                numberOrder2 = Sum(numberOrder2, "1");
                if (flag)
                {
                    mantissa2 = "1" + mantissa2;
                    flag = false;
                }
                else
                {
                    mantissa2 = "0" + mantissa2;
                }

                mantissa2 = mantissa2.Substring(0, 23);
                Console.WriteLine(numberSign2 + "|" + numberOrder2 + "|" + mantissa2);
            }

            binaryFloatNumber2 = numberSign2 + "|" + numberOrder2 + "|" + mantissa2;
        }

        private static string ConverterFromFloatToBinaryFloatStart()
        {
            Console.Write("Введите вещественное число ");
            string readLine = Console.ReadLine().Trim();

            if (double.TryParse(readLine, out double doubleNumber1))
                doubleNumber1 = doubleNumber1;
            else
                throw new ArgumentException("Ваше число некорректно");

            string Result = ConverterFromFloatToBinaryFloat(doubleNumber1);

            return Result;
        }

        private static string ConverterFromFloatToBinaryFloat(double doubleNumber1)
        {
            string str = Convert.ToString(doubleNumber1);
            string[] parts = str.Split(',');

            int intIntegerPartNumber = int.Parse(parts[0]);
            string floatPartNumber;
            if (parts.Length == 1)
            {
                floatPartNumber = "0";
            }
            else
            {
                floatPartNumber = parts[1];
            }

            string stringDoubleNumber1 = intIntegerPartNumber.ToString() + "," + floatPartNumber;
            Console.WriteLine();
            Console.WriteLine("Переведём вещественное число {0} в двоичную систему счисления", stringDoubleNumber1);
            Console.WriteLine();
            Console.WriteLine("Сначала переведём целую часть числа в двоичную систему счисления: ");

            string binaryIntegerPartNumber = IntegerPartNumberToBinary(stringDoubleNumber1, intIntegerPartNumber);
            int lenOfbinaryIntegerPartNumber;
            if (doubleNumber1 < 0 && intIntegerPartNumber == 0)
            {
                binaryIntegerPartNumber = "-" + binaryIntegerPartNumber;
            }
            if (binaryIntegerPartNumber.Substring(0, 1) == "-")
            {
                lenOfbinaryIntegerPartNumber = binaryIntegerPartNumber.Length - 1;
            }
            else
            {
                lenOfbinaryIntegerPartNumber = binaryIntegerPartNumber.Length;
            }
            Console.WriteLine();
            Console.WriteLine("Теперь переведём дробную часть числа в двоичную систему счисления: ");
            Console.WriteLine();
            string resultStringBinaryFloatPartNumber = FloatPartNumberToBinary(stringDoubleNumber1, ref floatPartNumber, lenOfbinaryIntegerPartNumber);

            Console.WriteLine();
            Console.WriteLine("Итого, число {0} в двоичной системе имеет вид: {1},{2}", stringDoubleNumber1, binaryIntegerPartNumber, resultStringBinaryFloatPartNumber);
            Console.ResetColor();
            string resultBinaryNumber = binaryIntegerPartNumber + resultStringBinaryFloatPartNumber;
            Console.WriteLine();
            Console.WriteLine("Теперь осталось только перевести полученное число в формат со смещенным порядком и мантиссой:");
            Console.WriteLine();
            Console.WriteLine("Для начала представим двоичное число в нормализованной экспоненциальной форме:");

            string numberSign = "";
            string mantissa = "";
            int numberOrder = 0;

            if (intIntegerPartNumber != 0)
            {
                numberSign = resultBinaryNumber.Substring(0, 1 + binaryIntegerPartNumber.Length - lenOfbinaryIntegerPartNumber);
                mantissa = resultBinaryNumber.Substring(1 + binaryIntegerPartNumber.Length - lenOfbinaryIntegerPartNumber);
                numberOrder = lenOfbinaryIntegerPartNumber - 1;
            }
            else
            {

                numberSign = resultStringBinaryFloatPartNumber.TrimStart('0').Substring(0, 1);
                if (doubleNumber1 < 0)
                {
                    numberSign = "-" + numberSign;
                }
                mantissa = resultStringBinaryFloatPartNumber.TrimStart('0').Substring(1);

                numberOrder = -(resultStringBinaryFloatPartNumber.Length - mantissa.Length);

                if (mantissa.Length == 0)
                {
                    mantissa = "0";
                }
            }

            Console.WriteLine("Это будет: {0},{1} * 2^{2}", numberSign, mantissa, numberOrder);
            Console.WriteLine();

            Console.WriteLine("Теперь осталось только вычислить смещенный порядок, и перевести его в двоичную систему счисления");
            int shiftedNumberOrder = 127 + numberOrder;
            Console.WriteLine("Значение смещенного порядка в десятичной системе счисления: 127 + {0} = {1}", numberOrder, shiftedNumberOrder);
            Console.WriteLine();
            Console.WriteLine("Теперь переведём значение смещенного порядка из десятичной системы счисления в двоичную систему счисления:");

            string binaryShiftedNumberOrder = RightFromDecToBinary(shiftedNumberOrder);
            Console.WriteLine($"{binaryShiftedNumberOrder}");
            Console.WriteLine();
            Console.WriteLine("Теперь осталось только составить представление числа {0} в формате нормализованной записи", stringDoubleNumber1);
            Console.WriteLine();
            string Result = "";
            if (doubleNumber1 < 0)
            {
                Console.WriteLine("Так как наше число отрицательное, то первой цифрой в числе будет 1");
                Console.WriteLine("После 1 пишем значение смещенного порядка в двоичной системе счисления: {0}", binaryShiftedNumberOrder.TrimStart('-'));
                Console.WriteLine("Ну и после этого пишем мантису: {0}", mantissa.PadRight(23, '0'));
                Console.WriteLine("(в случае если значение мантисы по длине меньше 23, то добавляем справа 0, пока длина не станет равной 23)");

                Result = "1" + "|" + binaryShiftedNumberOrder.TrimStart('-').PadLeft(8, '0') + "|" + mantissa.PadRight(23, '0');
            }
            else
            {
                Console.WriteLine("Так как наше число положительное, то первой цифрой в числе будет 0");
                Console.WriteLine("После 0 пишем значение смещенного порядка в двоичной системе счисления: {0}", binaryShiftedNumberOrder.TrimStart('-'));
                Console.WriteLine("Затем пишем мантису: {0}", mantissa.PadRight(23, '0'));
                Console.WriteLine("(в случае если значение мантисы по длине меньше 23, то добавляем справа 0, пока длина не станет равной 23)");
                Result = "0" + "|" + binaryShiftedNumberOrder.TrimStart('-').PadLeft(8, '0') + "|" + mantissa.PadRight(23, '0');
            }

            Console.WriteLine();
            Console.WriteLine("Представление числа {0} в формате нормализованной записи имеет вид {1}", stringDoubleNumber1, Result);
            Console.ResetColor();
            Console.WriteLine();
            return Result;
        }

        private static string FloatPartNumberToBinary(string stringDoubleNumber1, ref string stringFloatPartNumber, int lenOfbinaryIntegerPartNumber)
        {
            Console.WriteLine("Чтобы перевести число 0,{0} из десятичной системы счисления в двоичную будем умножать число на 2 и", stringFloatPartNumber);
            Console.WriteLine("записывать получившуюся целую часть, пока число 0,{0} не станет целым", stringFloatPartNumber);
            Console.WriteLine();
            int lenOfFloatPartNumber = stringFloatPartNumber.Length;
            int floatPartNumber = int.Parse(stringFloatPartNumber);
            StringBuilder resultBinaryFloatPartNumber = new StringBuilder();
            bool flag = true;
            for (int i = 0; i < 23 - lenOfbinaryIntegerPartNumber + 1; i++)
            {
                Console.WriteLine("0|{0}", floatPartNumber.ToString().PadLeft(lenOfFloatPartNumber, '0'));
                Console.WriteLine("*");
                Console.WriteLine(" |" + "2".PadLeft(lenOfFloatPartNumber, ' '));
                Console.WriteLine("".PadRight(lenOfFloatPartNumber + 2, '-'));
                floatPartNumber = floatPartNumber * 2;
                string floatStringPartNumber = floatPartNumber.ToString().PadLeft(lenOfFloatPartNumber + 1, '0');


                string stringPartBeforeI = floatStringPartNumber.Substring(0, 1);
                string stringPartAfterI = floatStringPartNumber.Substring(1);
                resultBinaryFloatPartNumber.Append(stringPartBeforeI);
                if (stringPartBeforeI == "0" && flag)
                {
                    lenOfbinaryIntegerPartNumber -= 1;
                }
                if (stringPartBeforeI == "1" && flag)
                {
                    lenOfbinaryIntegerPartNumber -= 1;
                    flag = false;
                }
                Console.Write(stringPartBeforeI);
                Console.ResetColor();


                Console.WriteLine("|" + stringPartAfterI);
                floatPartNumber = int.Parse(stringPartAfterI);
                Console.WriteLine();

                if (stringPartAfterI == "".PadLeft(lenOfFloatPartNumber, '0'))
                {
                    break;
                }
            }

            string resultStringBinaryFloatPartNumber = resultBinaryFloatPartNumber.ToString();
            Console.WriteLine();
            Console.WriteLine($"Дробная часть числа {stringDoubleNumber1} => 0,{stringDoubleNumber1.Split(',')[1]}  в двоичной системе имеет вид: 0,{resultStringBinaryFloatPartNumber}");
            Console.ResetColor();
            return resultStringBinaryFloatPartNumber;
        }

        private static string IntegerPartNumberToBinary(string stringDoubleNumber1, int intIntegerPartNumber)
        {
            bool ifLessThanZero = false;
            if (intIntegerPartNumber < 0)
            {
                Console.WriteLine();
                Console.WriteLine("Так как число {0} отрицательное, то переведем в двоичную систему модуль этого числа", intIntegerPartNumber);

                ifLessThanZero = true;
            }

            intIntegerPartNumber = Math.Abs(intIntegerPartNumber);

            string binaryIntegerPartNumber = RightFromDecToBinary(intIntegerPartNumber);

            if (ifLessThanZero)
            {
                Console.WriteLine();
                Console.WriteLine("Так как число -{0} отрицательное допишем слева \"-\"", intIntegerPartNumber);
                binaryIntegerPartNumber = "-" + binaryIntegerPartNumber;
            }
            Console.WriteLine();
            Console.WriteLine($"Целая часть числа {stringDoubleNumber1} => {stringDoubleNumber1.Split(',')[0]}  в двоичной системе имеет вид: {binaryIntegerPartNumber}");
            Console.ResetColor();
            return binaryIntegerPartNumber;
        }

        private static void AdditionalSumStart()
        {
            Console.Write("Введите первое целое число: ");
            string sumReadLine1 = Console.ReadLine();
            Console.Write("Введите второе целое число: ");
            string sumReadLine2 = Console.ReadLine();

            if (int.TryParse(sumReadLine1, out int number1))
                number1 = number1;
            else
                throw new ArgumentException("Первое число некоректно");

            if (int.TryParse(sumReadLine2, out int number2))
                number2 = number2;
            else
                throw new ArgumentException("Второе число некоректно");

            Console.WriteLine();
            Console.WriteLine("Переведём числа в дополнительный код:");

            string additionalNumber1 = ConverterToAdditional(number1);
            string additionalNumber2 = ConverterToAdditional(number2);
            Console.WriteLine();
            Console.WriteLine("Теперь производим суммирование двух чисел в дополнительном коде:");
            Console.WriteLine();
            string sum = CorrectSum(additionalNumber1, additionalNumber2);
            Console.WriteLine();
            Console.WriteLine("Итоговая сумма в дополнительном коде: {0}", sum);
            Console.WriteLine();

            if (sum.Length > 8)
            {
                sum = sum.Substring(1, 8);
            }

            if (sum.Substring(0, 1) == "1")
            {
                Console.WriteLine("Так как у нас первой цифрой стоит единица, это значит, что число представлено в дополнительном коде \n(необходимо отнять единицу и инвертировать)");
                Console.WriteLine();
                Console.WriteLine("Вычитаем единицу:");
                string sum1 = CorrectVichet(sum, "1");
                StringBuilder reverseSum1 = new StringBuilder();
                foreach (char c in sum1)
                {
                    if (c == '0')
                    {
                        reverseSum1.Append(1);
                    }
                    else
                    {
                        reverseSum1.Append(0);
                    }
                }
                Console.WriteLine();
                Console.WriteLine($"Инвертируем: {sum1} => {reverseSum1}");
                Console.WriteLine();
                Console.WriteLine($"Перeводим число {reverseSum1} из двоичной системы счисления в десятичную систему счисления:");


                int res1 = RightFromAnyToDec(reverseSum1.ToString(), 2);
                Console.WriteLine();
                Console.WriteLine("Так как число представляет модуль изначального числа, то необходимо взять противоположное число (отрицательное)");
                Console.WriteLine();
                Console.WriteLine("Итоговая сумма: -{0}", res1);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Так как у нас первой цифрой стоит ноль, это значит, что число представлено в двоичной системе счисления \nи его нужно перевести в десятичную систему счисления");

                int res1 = RightFromAnyToDec(sum, 2);
                Console.WriteLine();
                Console.WriteLine("Итоговая сумма: {0}", res1);

            }

        }


        private static string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static string CorrectVichet(string number1, string number2)
        {

            int baze = 2;
            int NumD1 = FromAnyToDec(number1, baze);
            int NumD2 = FromAnyToDec(number2, baze);

            string num1 = number1;
            string num2 = number2;
            if (NumD1 > NumD2)
            {
                number1 = num1;
                number2 = num2;
            }
            else
            {
                number1 = num2;
                number2 = num1;
            }

            number2 = number2.PadLeft(number1.Length, '0');
            List<char> charList1 = number1.ToCharArray().ToList();
            List<char> charList2 = number2.ToCharArray().ToList();
            //Console.Write(" ");
            //Console.WriteLine(number1);
            //Console.WriteLine("-");
            //Console.Write(" ");
            for (int i = 0; i < charList1.Count - charList2.Count; i++)
            {
                //Console.Write("0");
            }
            //Console.WriteLine(number2);
            //Console.Write(" ");
            for (int i = 0; i < charList1.Count; i++)
            {
                //Console.Write("-");
            }
           // Console.WriteLine();

            List<int> numberList1 = charList1.Select(c => (int)Alphabet.IndexOf(c)).ToList();
            List<int> numberList2 = charList2.Select(c => (int)Alphabet.IndexOf(c)).ToList();
            int j;


            for (int i = numberList1.Count - 1; i >= 0; i--)
            {
                j = i - (numberList1.Count - numberList2.Count);

                if (j >= 0)
                {
                    numberList1[i] -= numberList2[j];
                }

                while (numberList1[i] < 0)
                {
                    numberList1[i] += baze;
                    numberList1[i - 1]--;
                    //Console.WriteLine(String.Join(" ", numberList1));
                }
            }
            if (numberList1.Count == numberList2.Count)
            {
                //Console.WriteLine(String.Join(" ", numberList1));
            }
            Console.WriteLine();
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < numberList1.Count; i++)
            {
                result.Append(Alphabet[numberList1[i]]);
            }
            Console.Write(" ");
            Console.WriteLine(number1);
            Console.WriteLine("-");
            Console.Write(" ");

            for (int i = 0; i < charList1.Count - charList2.Count; i++)
            {
                Console.Write("0");
            }
            Console.WriteLine(number2);
            Console.Write(" ");
            for (int i = 0; i < charList1.Count; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            Console.Write(" ");
            Console.WriteLine(result.ToString());
            return (result.ToString());
        }

        private static void ConverterToAdditionalStart()
        {
            Console.Write("Введите целое число для перевода в дополнительный код: ");

            string readLine = Console.ReadLine().Trim();

            if (int.TryParse(readLine, out int number1))
                number1 = number1;
            else
                throw new ArgumentException("Ваше число некорректно");

            ConverterToAdditional(number1);
        }

        private static string ConverterToAdditional(int number1)
        {
            int startNumber1 = number1;
            string binNumber;
            Console.WriteLine();
            if (number1 < -128)
            {
                throw new ArgumentException("Ваше число меньше -128");
            }

            if (number1 > 127)
            {
                throw new ArgumentException("Ваше число больше 127");
            }

            if (number1 >= 0)
            {
                Console.WriteLine("Число {0} - положительное, для перевода в дополнительный код нужно перевести в двоичную систему счисления.", number1);
                binNumber = RightFromDecToBinary(number1);
                binNumber = binNumber.PadLeft(8, '0');
            }

            else
            {
                Console.WriteLine($"Число {number1} - отрицательное, для перевода в дополнительный код нам необходимо взять модуль введеного числа => {Math.Abs(number1)}");
                number1 = Math.Abs(number1);

                binNumber = RightFromDecToBinary(number1);
                Console.WriteLine();
                binNumber = binNumber.PadLeft(8, '0');
                Console.WriteLine($"Переведём его в двоичную систему счисления и получим: {binNumber}");
                Console.WriteLine();
                Console.WriteLine("Необходимо значение всех бит инвертировать: все нули заменить на единицы, а единицы на нули ");

                StringBuilder reverseBinNumber = new StringBuilder();
                foreach (char c in binNumber)
                {
                    if (c == '0')
                    {
                        reverseBinNumber.Append(1);
                    }
                    else
                    {
                        reverseBinNumber.Append(0);
                    }
                }
                Console.WriteLine($"{binNumber} => {reverseBinNumber}");
                Console.WriteLine();
                Console.WriteLine("Теперь прибавим к числу {0} 1", reverseBinNumber);
                Console.WriteLine();
                binNumber = CorrectSum(reverseBinNumber.ToString(), "1");

            }
            Console.WriteLine();
            Console.WriteLine("Получаем число {0} в дополнительном коде: {1}", startNumber1, binNumber);
            return binNumber;
        }

        static int FromAnyToDec(string number, int baze)
        {

            long result = 0;
            int digitsCount = number.Length;
            int num;

            for (int i = 0; i < digitsCount; i++)
            {
                char c = number[i];
                num = c - '0';
                result *= baze;
                result += num;
            }

            return (int)result;
        }

        static int RightFromAnyToDec(string number, int baze)
        {

            if (baze > 50)
                throw new ArgumentException("Система счисления должна быть меньше или равна 50");
            long result = 0;
            int digitsCount = number.Length;
            int num;
            number = new string(number.Reverse().ToArray());
            for (int i = 0; i < digitsCount; i++)
            {
                char c = number[i];

                if (c >= '0' && c <= '9')
                    num = c - '0';
                else if (c >= 'A' && c <= 'Z')
                    num = c - 'A' + 10;
                else if (c >= 'a' && c <= 'z')
                    num = c - 'a' + (('Z' - 'A') + 1) + 10;
                else throw new ArgumentException("Строка содержит символ не корректный для данной системы счисления");

                if (num >= baze)
                    throw new ArgumentException("Строка содержит символ не корректный для данной системы счисления");


                result += num * (long)Math.Pow(baze, i);

                if (result > 2147483647)
                {
                    throw new ArgumentException("Ваше число слишком большое, см. примечание");
                }
            }
            Console.WriteLine($"{result}");
            Console.WriteLine();
            return (int)result;
        }
        public static string CorrectSum(string number1, string number2)
        {

            StringBuilder sum = new StringBuilder();

            string num1 = number1;
            string num2 = number2;

            int base1 = 2;

            int NumD1 = FromAnyToDec(num1, 2);
            int NumD2 = FromAnyToDec(num2, 2);
            int len = 0;
            string maxNum = "";
            string minNum = "";

            if (NumD1 > NumD2)
            {
                len = num1.Length;
                maxNum = num1;
                minNum = num2;
            }
            else
            {
                len = num2.Length;
                maxNum = num2;
                minNum = num1;
            }
            maxNum = new string(maxNum.Reverse().ToArray());
            minNum = new string(minNum.Reverse().ToArray());
            int des = 0;
            for (int i = 0; i < len; i++)
            {
                int res = 0;
                int digit1 = maxNum[i] - '0';
                int digit2 = 0;
                if (i < minNum.Length)
                    digit2 = minNum[i] - '0';

                res = des + digit1 + digit2;

                if (res >= base1)
                {

                    sum.Append(res - base1);
                    if (i == len - 1)
                    {
                        sum.Append("1");

                    }
                    else
                    {
                        des = 1;
                    }
                }
                else
                {
                    des = 0;
                    sum.Append(res);

                }
            }

            maxNum = new string(maxNum.Reverse().ToArray());
            minNum = new string(minNum.Reverse().ToArray());
            string res1 = sum.ToString();
            res1 = new string(res1.Reverse().ToArray());
            int lenRes = res1.Length;

            Console.Write(" ");
            Console.WriteLine(maxNum.PadLeft(lenRes));
            Console.WriteLine("+");
            Console.Write(" ");

            Console.WriteLine(minNum.PadLeft(lenRes));
            Console.Write(" ");
            for (int i = 0; i < lenRes; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            Console.Write(" ");
            Console.WriteLine(res1);

            return res1.ToString();
        }

        static string RightFromDecToBinary(int number)
        {
            int baze = 2;
            int numberStart = number;
            StringBuilder builder = new StringBuilder();
            do
            {
                int mod = number % baze;
                char c = (char)('0' + mod);
                builder.Append(c);
                number /= baze;
            } while (number >= baze);

            if (number != 0)
            {

                builder.Append((char)('0' + number));
            }
            string result = string.Join("", builder.ToString().Reverse());
            return result;
        }

        public static string Sum(string number1, string number2)
        {

            StringBuilder sum = new StringBuilder();

            string num1 = number1;
            string num2 = number2;

            int base1 = 2;

            int NumD1 = FromAnyToDec(num1, 2);
            int NumD2 = FromAnyToDec(num2, 2);
            int len = 0;
            string maxNum = "";
            string minNum = "";

            if (NumD1 > NumD2)
            {
                len = num1.Length;
                maxNum = num1;
                minNum = num2;
            }
            else
            {
                len = num2.Length;
                maxNum = num2;
                minNum = num1;
            }

            maxNum = new string(maxNum.Reverse().ToArray());
            minNum = new string(minNum.Reverse().ToArray());


            int des = 0;
            for (int i = 0; i < len; i++)
            {
                int res = 0;
                int digit1 = maxNum[i] - '0';
                int digit2 = 0;
                if (i < minNum.Length)
                    digit2 = minNum[i] - '0';

                res = des + digit1 + digit2;

                if (res >= base1)
                {

                    sum.Append(res - base1);
                    if (i == len - 1)
                    {
                        sum.Append("1");

                    }
                    else
                    {
                        des = 1;
                    }
                }
                else
                {
                    des = 0;
                    sum.Append(res);
                }
            }

            maxNum = new string(maxNum.Reverse().ToArray());
            minNum = new string(minNum.Reverse().ToArray());
            string res1 = sum.ToString();
            res1 = new string(res1.Reverse().ToArray());
            int lenRes = res1.Length;

            return res1.ToString();
        }
    }
}