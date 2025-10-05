export interface PostPrognozujEmerytureRequest {
  wiek: number;
  czyMezczyzna: boolean;
  oczekiwanaEmerytura: number;
  wiekPrzejsciaNaEmeryture: number;
  kapitalPoczatkowy: number;
  kodPocztowy: string;
  wskaznikWaloryzacjiKonta?: number | null;
  wskaznikWaloryzacjiSubkonta?: number | null;
  wynagrodzeniaBrutto: Record<number, number>;
}

export interface PrzewidywanaEmerytura {
  wysokoscRzeczywista: number;
  wysokoscUrealniona: number;
  naKoncie: number;
  naSubkoncie: number;
}

export interface PostPrognozujEmerytureResponse {
  przewidywanaEmerytura: Record<number, PrzewidywanaEmerytura>;
}

export async function postPrognozujEmeryture(
  data: PostPrognozujEmerytureRequest
): Promise<PostPrognozujEmerytureResponse> {
  const response = await fetch("/api/PostPrognozujEmeryture", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    throw new Error(`Błąd ${response.status}: Nie udało się pobrać prognozy emerytury`);
  }

  const result: PostPrognozujEmerytureResponse = await response.json();
  return result;
}