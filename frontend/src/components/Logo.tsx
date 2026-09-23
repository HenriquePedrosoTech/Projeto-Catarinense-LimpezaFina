import Image from "next/image";

export function Logo({ className = "" }: { className?: string }) {
  return (
    <div className={`flex items-center gap-3 ${className}`}>
      <div className="flex flex-col items-center justify-center shrink-0 pt-0.5">
        <Image 
          src="/logo.png" 
          alt="Logo Catarinense" 
          width={44} 
          height={44} 
          className="rounded object-contain"
          priority
        />
        <span className="text-[11px] font-bold tracking-widest uppercase text-ink mt-1 leading-none whitespace-nowrap">
          Catarinense
        </span>
      </div>
      <span className="text-xl font-light tracking-tight text-ink/70 border-l border-line pl-3 whitespace-nowrap h-10 flex items-center">
        Limpeza Fina
      </span>
    </div>
  );
}

export function LogoJCA() {
  return (
    <span className="text-xs text-ink/40">
      Sistema desenvolvido para o{" "}
      <a
        href="https://jcaholding.com.br/"
        target="_blank"
        rel="noreferrer"
        className="underline decoration-dotted hover:text-brand"
      >
        Grupo JCA
      </a>
    </span>
  );
}